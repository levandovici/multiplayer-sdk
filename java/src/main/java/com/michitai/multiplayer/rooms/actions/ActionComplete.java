package com.michitai.multiplayer.rooms.actions;

/**
 * Request parameters for completing an action with a response.
 *
 * @param <T> The type of response data.
 */
public class ActionComplete<T> {
    private ERoomCompleteActionStatus status;
    private T responseData;

    public ActionComplete(ERoomCompleteActionStatus status, T responseData) {
        this.status = status;
        this.responseData = responseData;
    }

    public ERoomCompleteActionStatus getStatus() {
        return status;
    }

    public void setStatus(ERoomCompleteActionStatus status) {
        this.status = status;
    }

    public T getResponseData() {
        return responseData;
    }

    public void setResponseData(T responseData) {
        this.responseData = responseData;
    }
}
