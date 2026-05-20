using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Errors
{
    /// <summary>
    /// Interface for logging API requests, responses, and errors.
    /// Implement this interface to provide custom logging behavior.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs a general information message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void Log(string message);

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The warning message to log.</param>
        void Warn(string message);

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">The error message to log.</param>
        void Error(string message);
    }
}
