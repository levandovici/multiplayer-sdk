package com.michitai.multiplayer.rooms.realtime;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.michitai.multiplayer.Client;

import java.io.IOException;
import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.net.http.WebSocket;
import java.nio.ByteBuffer;
import java.util.concurrent.CompletableFuture;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.TimeUnit;
import java.util.concurrent.atomic.AtomicBoolean;

/**
 * Manages WebSocket connections for realtime communication in game rooms.
 * Handles connection, message sending/receiving, and automatic heartbeat.
 */
public class Realtime {
    private WebSocket webSocket;
    private String token;
    private CompletableFuture<Void> messageListenerFuture;
    private CompletableFuture<Void> heartbeatFuture;
    private final String realtimeWebSocketUrl;
    private final ObjectMapper objectMapper;
    private final AtomicBoolean isConnected = new AtomicBoolean(false);

    /**
     * Event raised when a message is received from the WebSocket.
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

    /**
     * Retrieves a realtime authentication token for WebSocket connections.
     *
     * @param client The API client instance.
     * @param playerToken The player's private authentication token.
     * @return Response containing the realtime token.
     * @throws IOException if the request fails.
     */
    public static TokenResponse getToken(Client client, String playerToken) throws IOException {
        String url = client.url(Endpoints.REALTIME_TOKEN, "&player_token=" + playerToken);
        return client.post(url, null, TokenResponse.class);
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

            URI uri = URI.create(realtimeWebSocketUrl + "?token=" + token + "&client=json");
            
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
     * Disconnects from the WebSocket server and cleans up resources.
     */
    public void disconnect() {
        try {
            if (heartbeatFuture != null) {
                heartbeatFuture.cancel(true);
            }
            if (messageListenerFuture != null) {
                messageListenerFuture.cancel(true);
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
            
            if ("receive".equals(realtimeMessage.getType()) && messageListener != null) {
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
        private int[] targetIds;
        private String target;

        public String getType() { return type; }
        public void setType(String type) { this.type = type; }
        public String getCommand() { return command; }
        public void setCommand(String command) { this.command = command; }
        public Object getData() { return data; }
        public void setData(Object data) { this.data = data; }
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
}
