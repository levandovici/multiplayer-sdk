package com.michitai.multiplayer.regions;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Realtime (WS/UDP) endpoint advertised by a service node.
 */
public class RealtimeEndpoint {
    @JsonProperty("host")
    private String host;

    @JsonProperty("port")
    private int port = 443;

    @JsonProperty("protocol")
    private String protocol = "wss";

    @JsonProperty("udp_port")
    private Integer udpPort;

    /** Realtime WebSocket host. */
    public String getHost() {
        return host;
    }

    public void setHost(String host) {
        this.host = host;
    }

    /** TLS port the wss:// proxy listens on. */
    public int getPort() {
        return port;
    }

    public void setPort(int port) {
        this.port = port;
    }

    /** WebSocket scheme — "wss" in production. */
    public String getProtocol() {
        return protocol;
    }

    public void setProtocol(String protocol) {
        this.protocol = protocol;
    }

    /** UDP fast-lane port; null when the node runs TCP-only. */
    public Integer getUdpPort() {
        return udpPort;
    }

    public void setUdpPort(Integer udpPort) {
        this.udpPort = udpPort;
    }
}
