package com.michitai.multiplayer.rooms.actions;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Request for completing an action with a response.
 */
public class ActionCompleteRequest {
    @JsonProperty("status")
    private String status;

    @JsonProperty("response_data")
    private String responseData;

    public ActionCompleteRequest(String status, String responseData) {
        this.status = status;
        this.responseData = responseData;
    }

    public String getStatus() {
        return status;
    }

    public void setStatus(String status) {
        this.status = status;
    }

    public String getResponseData() {
        return responseData;
    }

    public void setResponseData(String responseData) {
        this.responseData = responseData;
    }
}
