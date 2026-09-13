package com.michitai.multiplayer.errors;

/**
 * Default console-based logger implementation.
 * Outputs log messages to System.out and System.err with appropriate prefixes.
 */
public class ConsoleLogger implements Logger {
    @Override
    public void error(String message) {
        System.err.println("[SDK Error] " + message);
    }

    @Override
    public void log(String message) {
        System.out.println("[SDK] " + message);
    }

    @Override
    public void warn(String message) {
        System.out.println("[SDK Warning] " + message);
    }
}
