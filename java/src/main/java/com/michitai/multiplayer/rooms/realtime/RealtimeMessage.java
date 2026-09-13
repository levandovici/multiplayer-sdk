package com.michitai.multiplayer.rooms.realtime;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Realtime WebSocket message structure.
 */
public class RealtimeMessage {
    @JsonProperty("type")
    private String type;

    @JsonProperty("command")
    private String command;

    @JsonProperty("data")
    private Object data;

    @JsonProperty("sender")
    private SenderInfo sender;

    public String getType() {
        return type;
    }

    public void setType(String type) {
        this.type = type;
    }

    public String getCommand() {
        return command;
    }

    public void setCommand(String command) {
        this.command = command;
    }

    public Object getData() {
        return data;
    }

    public void setData(Object data) {
        this.data = data;
    }

    public SenderInfo getSender() {
        return sender;
    }

    public void setSender(SenderInfo sender) {
        this.sender = sender;
    }
}
