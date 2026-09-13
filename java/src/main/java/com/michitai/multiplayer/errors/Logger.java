package com.michitai.multiplayer.errors;

/**
 * Interface for logging API requests, responses, and errors.
 * Implement this interface to provide custom logging behavior.
 */
public interface Logger {
    /**
     * Logs a general information message.
     *
     * @param message The message to log.
     */
    void log(String message);

    /**
     * Logs a warning message.
     *
     * @param message The warning message to log.
     */
    void warn(String message);

    /**
     * Logs an error message.
     *
     * @param message The error message to log.
     */
    void error(String message);
}
