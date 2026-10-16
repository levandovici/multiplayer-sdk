package com.michitai.multiplayer.rooms.realtime;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Information about the realtime WebSocket server.
 * Contains connection details for establishing WebSocket connections.
 */
public class RealtimeServerInfo {
    @JsonProperty("host")
    private String host;

    @JsonProperty("port")
    private int port;

    @JsonProperty("protocol")
    private String protocol;

    /** The UDP port for the unreliable datagram channel (0 = unavailable). */
    @JsonProperty("udp_port")
    private int udpPort;

    public String getHost() {
        return host;
    }

    public void setHost(String host) {
        this.host = host;
    }

    public int getPort() {
        return port;
    }

    public void setPort(int port) {
        this.port = port;
    }

    public String getProtocol() {
        return protocol;
    }

    public void setProtocol(String protocol) {
        this.protocol = protocol;
    }

    public int getUdpPort() {
        return udpPort;
    }

    public void setUdpPort(int udpPort) {
        this.udpPort = udpPort;
    }
}
