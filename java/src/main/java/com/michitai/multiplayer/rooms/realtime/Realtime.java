package com.michitai.multiplayer.rooms.realtime;

import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.michitai.multiplayer.Client;

import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.InetSocketAddress;
import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.net.http.WebSocket;
import java.nio.ByteBuffer;
import java.nio.charset.StandardCharsets;
import java.util.UUID;
import java.util.concurrent.CompletableFuture;
import java.util.concurrent.TimeUnit;
import java.util.concurrent.atomic.AtomicBoolean;
import java.util.concurrent.atomic.AtomicInteger;
import java.util.concurrent.atomic.AtomicLong;

/**
 * Manages realtime communication in game rooms over two channels:
 * a reliable WebSocket (TCP) connection for control traffic plus one
 * optional bound UDP address for high-frequency unreliable traffic
 * (movement state, inputs). One player may hold one TCP connection
 * and one UDP binding at the same time — see {@link #connectUdp()}.
 */
public class Realtime {
    private WebSocket webSocket;
    private String token;
    private CompletableFuture<Void> messageListenerFuture;
    private CompletableFuture<Void> heartbeatFuture;
    private final String realtimeWebSocketUrl;
    private final ObjectMapper objectMapper;
    private final AtomicBoolean isConnected = new AtomicBoolean(false);

    // UDP companion channel state
    private DatagramSocket udpSocket;
    private Thread udpReaderThread;
    private final AtomicBoolean udpBound = new AtomicBoolean(false);
    private volatile String connectionId;
    private final AtomicInteger serverUdpPort = new AtomicInteger(0);
    private final AtomicLong udpSeq = new AtomicLong(0);
    private volatile int lastUdpRttMs = 0;

    /**
     * Event raised when a message is received (either transport).
     */
    public interface MessageListener {
        void onMessage(String command, Object data, SenderInfo sender);
    }

    /**
     * Event raised when the WebSocket connection is established.
     */
    public interface ConnectionListener {
        void onConnected();
    }

    private MessageListener messageListener;
    private ConnectionListener connectionListener;

    /**
     * Initializes a new Realtime instance.
     *
     * @param realtimeWebSocketUrl The WebSocket server URL (default: wss://realtime.michitai.com).
     */
    public Realtime(String realtimeWebSocketUrl) {
        this.realtimeWebSocketUrl = realtimeWebSocketUrl != null ? realtimeWebSocketUrl : "wss://realtime.michitai.com";
        this.objectMapper = Client.JSON_MAPPER;
    }

    public Realtime() {
        this("wss://realtime.michitai.com");
    }

    /**
     * Sets the message listener for received messages.
     */
    public void setMessageListener(MessageListener listener) {
        this.messageListener = listener;
    }

    /**
     * Sets the connection listener for connection events.
     */
    public void setConnectionListener(ConnectionListener listener) {
        this.connectionListener = listener;
    }

    /** The connection id issued by the server, or null before connect. */
    public String getConnectionId() {
        return connectionId;
    }

    /** True while a UDP address is bound to this session. */
    public boolean isUdpConnected() {
        return udpBound.get() && udpSocket != null;
    }

    /** Last measured UDP round-trip time in milliseconds (0 = unknown). */
    public int getLastUdpRttMs() {
        return lastUdpRttMs;
    }

    /**
     * Retrieves a realtime authentication token for WebSocket connections.
     *
     * @param client The API client instance.
     * @param playerToken The player's private authentication token.
     * @return Response containing the realtime token.
     * @throws IOException if the request fails.
     */
    public static TokenResponse getToken(Client client, String playerToken) throws IOException {
        String url = client.url(Endpoints.REALTIME_TOKEN);
        return client.send("POST", url, null, TokenResponse.class, playerToken, false);
    }

    /**
     * Connects to the realtime WebSocket server using the provided token.
     *
     * @param realtimeToken The realtime authentication token.
     * @return True if connection succeeded, false otherwise.
     */
    public boolean connect(String realtimeToken) {
        try {
            this.token = realtimeToken;
            isConnected.set(false);

            // Wake up the server before connecting
            HttpClient httpClient = HttpClient.newHttpClient();
            HttpRequest wakeRequest = HttpRequest.newBuilder()
                .uri(URI.create("https://realtime.michitai.com/"))
                .GET()
                .build();
            httpClient.send(wakeRequest, HttpResponse.BodyHandlers.ofString());

            // Token goes in a header so it stays out of URLs/logs.
            URI uri = URI.create(realtimeWebSocketUrl + "?client=json");

            WebSocket.Listener listener = new WebSocket.Listener() {
                private StringBuilder messageBuilder = new StringBuilder();

                @Override
                public CompletableFuture<Void> onText(WebSocket webSocket, CharSequence data, boolean last) {
                    messageBuilder.append(data);
                    if (last) {
                        String message = messageBuilder.toString();
                        messageBuilder.setLength(0);
                        handleMessage(message);
                    }
                    return CompletableFuture.completedFuture(null);
                }

                @Override
                public void onOpen(WebSocket webSocket) {
                    isConnected.set(true);
                    if (connectionListener != null) {
                        connectionListener.onConnected();
                    }
                    webSocket.request(1);
                }
            };

            webSocket = HttpClient.newHttpClient()
                .newWebSocketBuilder()
                .header("X-Realtime-Token", token)
                .buildAsync(uri, listener)
                .join();

            // Start message listener
            messageListenerFuture = CompletableFuture.runAsync(() -> {
                while (isConnected.get()) {
                    try {
                        TimeUnit.MILLISECONDS.sleep(100);
                    } catch (InterruptedException e) {
                        break;
                    }
                }
            });

            // Start heartbeat
            heartbeatFuture = CompletableFuture.runAsync(this::heartbeatLoop);

            return true;
        } catch (Exception ex) {
            System.err.println("Connection failed: " + ex.getMessage());
            disconnect();
            return false;
        }
    }

    /**
     * Binds a UDP endpoint to this session. Call after {@link #connect}; the
     * server accepts one UDP binding per connection. Datagrams are
     * authenticated with the connection id issued on the socket.
     *
     * @param udpPort Override port — pass null to use the udp_port
     *                announced in the server's connected payload.
     * @return True when the server acknowledged the binding.
     */
    public boolean connectUdp(Integer udpPort) {
        if (!isConnected.get() || connectionId == null) return false;
        if (udpBound.get()) return true;

        try {
            URI uri = URI.create(realtimeWebSocketUrl);
            // The WS port is a proxy port for wss:// — UDP needs the
            // service port announced by the server (3000 by default).
            int port = udpPort != null ? udpPort
                : (serverUdpPort.get() > 0 ? serverUdpPort.get() : 3000);

            udpSocket = new DatagramSocket();
            udpSocket.connect(new InetSocketAddress(uri.getHost(), port));

            udpReaderThread = new Thread(this::udpReadLoop, "realtime-udp-reader");
            udpReaderThread.setDaemon(true);
            udpReaderThread.start();

            // BIND is unreliable — retry once before giving up.
            byte[] bindPacket = UdpWire.encode(UdpWire.TYPE_BIND, connectionId, "{\"v\":1}");
            InetAddress host = InetAddress.getByName(uri.getHost());
            for (int attempt = 0; attempt < 2 && !udpBound.get(); attempt++) {
                udpSocket.send(new DatagramPacket(bindPacket, bindPacket.length, host, port));
                long deadline = System.currentTimeMillis() + 1500;
                while (!udpBound.get() && System.currentTimeMillis() < deadline) {
                    Thread.sleep(25);
                }
            }
            return udpBound.get();
        } catch (Exception ex) {
            System.err.println("UDP connect failed: " + ex.getMessage());
            return false;
        }
    }

    /**
     * Binds a UDP endpoint using the server-announced port.
     */
    public boolean connectUdp() {
        return connectUdp(null);
    }

    /**
     * Sends a message to the specified players via WebSocket.
     *
     * @param target The target players (ALL, HOST, OTHERS, SPECIFIC).
     * @param command The command/type of the message.
     * @param data Optional data payload to send.
     * @param targetIds Specific player IDs if target is SPECIFIC.
     */
    public void send(ERoomTargetPlayer target, String command, Object data, int[] targetIds) {
        if (!isConnected.get() || webSocket == null) return;

        try {
            SendMessage message = new SendMessage();
            message.setType("send");
            message.setCommand(command);
            message.setData(data);
            message.setTargetIds(targetIds != null ? targetIds : new int[0]);
            message.setTarget(target.name().toLowerCase());

            String json = objectMapper.writeValueAsString(message);
            webSocket.sendText(json, true);
        } catch (Exception ex) {
            System.err.println("Send error: " + ex.getMessage());
        }
    }

    /**
     * Sends over the bound UDP channel — unordered, unacknowledged,
     * latest-wins via seq. Best for movement/state streams.
     * Falls back to the WebSocket when UDP is not bound.
     */
    public void sendUnreliable(ERoomTargetPlayer target, String command, Object data, int[] targetIds) {
        if (!udpBound.get() || udpSocket == null || connectionId == null) {
            send(target, command, data, targetIds);
            return;
        }

        try {
            SendMessage message = new SendMessage();
            message.setType("send");
            message.setCommand(command);
            message.setData(data);
            message.setSeq(udpSeq.incrementAndGet());
            message.setTargetIds(targetIds != null ? targetIds : new int[0]);
            message.setTarget(target.name().toLowerCase());

            byte[] packet = UdpWire.encode(UdpWire.TYPE_MSG, connectionId,
                objectMapper.writeValueAsString(message));
            udpSocket.send(new DatagramPacket(packet, packet.length));
        } catch (Exception ex) {
            System.err.println("UDP send error: " + ex.getMessage());
        }
    }

    /**
     * Disconnects from the server and cleans up both channels.
     */
    public void disconnect() {
        try {
            udpBound.set(false);
            if (heartbeatFuture != null) {
                heartbeatFuture.cancel(true);
            }
            if (messageListenerFuture != null) {
                messageListenerFuture.cancel(true);
            }
            if (udpSocket != null) {
                udpSocket.close();
                udpSocket = null;
            }

            if (webSocket != null) {
                webSocket.sendClose(WebSocket.NORMAL_CLOSURE, "Client disconnecting").join();
            }
        } catch (Exception ex) {
            System.err.println("Disconnect error: " + ex.getMessage());
        } finally {
            isConnected.set(false);
        }
    }

    private void handleMessage(String message) {
        try {
            RealtimeMessage realtimeMessage = objectMapper.readValue(message, RealtimeMessage.class);

            if ("connected".equals(realtimeMessage.getType())) {
                JsonNode data = objectMapper.valueToTree(realtimeMessage.getData());
                if (data != null && data.isObject()) {
                    JsonNode cid = data.get("connection_id");
                    if (cid != null) connectionId = cid.asText();
                    JsonNode up = data.get("udp_port");
                    if (up != null && up.isNumber()) serverUdpPort.set(up.asInt());
                }
            } else if ("receive".equals(realtimeMessage.getType()) && messageListener != null) {
                messageListener.onMessage(
                    realtimeMessage.getCommand() != null ? realtimeMessage.getCommand() : "",
                    realtimeMessage.getData() != null ? realtimeMessage.getData() : new Object(),
                    realtimeMessage.getSender()
                );
            }
        } catch (Exception ex) {
            System.err.println("Message handling error: " + ex.getMessage());
        }
    }

    private void udpReadLoop() {
        byte[] buf = new byte[2048];
        while (udpSocket != null && !udpSocket.isClosed()) {
            try {
                DatagramPacket packet = new DatagramPacket(buf, buf.length);
                udpSocket.receive(packet);

                String[] connIdOut = new String[1];
                byte type = UdpWire.decodeType(packet.getData(), packet.getLength(), connIdOut);
                if (type == 0 || !connectionId.equals(connIdOut[0])) continue;

                String json = UdpWire.decodeBody(packet.getData(), packet.getLength());

                switch (type) {
                    case UdpWire.TYPE_BIND_ACK: {
                        JsonNode ack = objectMapper.readTree(json);
                        if (ack.has("ok") && ack.get("ok").asBoolean()) {
                            udpBound.set(true);
                        }
                        break;
                    }
                    case UdpWire.TYPE_MSG: {
                        RealtimeMessage msg = objectMapper.readValue(json, RealtimeMessage.class);
                        if ("receive".equals(msg.getType()) && messageListener != null) {
                            messageListener.onMessage(
                                msg.getCommand() != null ? msg.getCommand() : "",
                                msg.getData() != null ? msg.getData() : new Object(),
                                msg.getSender()
                            );
                        }
                        break;
                    }
                    case UdpWire.TYPE_PONG: {
                        JsonNode pong = objectMapper.readTree(json);
                        JsonNode t = pong.get("t");
                        if (t != null && t.isNumber()) {
                            lastUdpRttMs = Math.max(0, (int) (System.currentTimeMillis() - t.asLong()));
                        }
                        break;
                    }
                    default:
                        break;
                }
            } catch (Exception ex) {
                if (udpSocket == null || udpSocket.isClosed()) break;
            }
        }
    }

    private void heartbeatLoop() {
        while (isConnected.get()) {
            try {
                TimeUnit.SECONDS.sleep(20);

                if (isConnected.get() && webSocket != null) {
                    sendHeartbeat();
                }
            } catch (InterruptedException e) {
                break;
            } catch (Exception ex) {
                System.err.println("Heartbeat error: " + ex.getMessage());
            }
        }
    }

    private void sendHeartbeat() {
        try {
            HeartbeatMessage message = new HeartbeatMessage();
            message.setType("heartbeat");

            String json = objectMapper.writeValueAsString(message);
            webSocket.sendText(json, true);

            // Keep the NAT mapping warm on the UDP channel too.
            if (udpBound.get() && udpSocket != null && connectionId != null) {
                byte[] ping = UdpWire.encode(UdpWire.TYPE_PING, connectionId,
                    "{\"t\":" + System.currentTimeMillis() + "}");
                udpSocket.send(new DatagramPacket(ping, ping.length));
            }
        } catch (Exception ex) {
            System.err.println("Send heartbeat error: " + ex.getMessage());
        }
    }

    /**
     * Inner class for send messages.
     */
    private static class SendMessage {
        private String type;
        private String command;
        private Object data;
        private long seq;
        private int[] targetIds;
        private String target;

        public String getType() { return type; }
        public void setType(String type) { this.type = type; }
        public String getCommand() { return command; }
        public void setCommand(String command) { this.command = command; }
        public Object getData() { return data; }
        public void setData(Object data) { this.data = data; }
        public long getSeq() { return seq; }
        public void setSeq(long seq) { this.seq = seq; }
        public int[] getTargetIds() { return targetIds; }
        public void setTargetIds(int[] targetIds) { this.targetIds = targetIds; }
        public String getTarget() { return target; }
        public void setTarget(String target) { this.target = target; }
    }

    /**
     * Inner class for heartbeat messages.
     */
    private static class HeartbeatMessage {
        private String type;

        public String getType() { return type; }
        public void setType(String type) { this.type = type; }
    }

    public boolean isConnected() {
        return isConnected.get();
    }

    /**
     * MUR1 datagram codec — [4B magic][1B type][16B connection id][JSON].
     * Shared layout with realtime/src/protocol.js.
     */
    static final class UdpWire {
        static final int HEADER_SIZE = 21;
        static final byte TYPE_BIND = 1;
        static final byte TYPE_BIND_ACK = 2;
        static final byte TYPE_MSG = 3;
        static final byte TYPE_PING = 4;
        static final byte TYPE_PONG = 5;

        private UdpWire() {}

        static byte[] encode(byte type, String connectionId, String jsonBody) {
            UUID uuid = UUID.fromString(connectionId);
            byte[] body = jsonBody == null ? new byte[0] : jsonBody.getBytes(StandardCharsets.UTF_8);
            ByteBuffer buf = ByteBuffer.allocate(HEADER_SIZE + body.length);
            buf.putInt(0x4D555231); // 'MUR1'
            buf.put(type);
            buf.putLong(uuid.getMostSignificantBits());
            buf.putLong(uuid.getLeastSignificantBits());
            buf.put(body);
            return buf.array();
        }

        /** Returns the packet type (0 = not ours) and writes the connection id to out[0]. */
        static byte decodeType(byte[] datagram, int length, String[] out) {
            if (datagram == null || length < HEADER_SIZE) return 0;
            ByteBuffer buf = ByteBuffer.wrap(datagram, 0, length);
            if (buf.getInt() != 0x4D555231) return 0;
            byte type = buf.get();
            long hi = buf.getLong();
            long lo = buf.getLong();
            if (out != null && out.length > 0) {
                out[0] = new UUID(hi, lo).toString();
            }
            return type;
        }

        static String decodeBody(byte[] datagram, int length) {
            if (length <= HEADER_SIZE) return null;
            return new String(datagram, HEADER_SIZE, length - HEADER_SIZE, StandardCharsets.UTF_8);
        }
    }
}
