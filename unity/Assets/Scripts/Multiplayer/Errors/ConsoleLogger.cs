using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Michitai.Multiplayer.Errors
{
    /// <summary>
    /// Default console-based logger implementation for Unity.
    /// Outputs log messages to Unity's Debug console with appropriate prefixes.
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        /// <summary>
        /// Logs an error message to Unity's Debug console.
        /// </summary>
        /// <param name="message">The error message to log.</param>
        public void Error(string message) => Debug.LogError($"[SDK Error] {message}");

        /// <summary>
        /// Logs a general information message to Unity's Debug console.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Log(string message) => Debug.Log($"[SDK] {message}");

        /// <summary>
        /// Logs a warning message to Unity's Debug console.
        /// </summary>
        /// <param name="message">The warning message to log.</param>
        public void Warn(string message) => Debug.LogWarning($"[SDK Warning] {message}");
    }
}
