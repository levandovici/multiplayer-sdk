using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Errors
{
    /// <summary>
    /// Default console-based logger implementation.
    /// Outputs log messages to the console with appropriate prefixes.
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        /// <summary>
        /// Logs an error message to the console.
        /// </summary>
        /// <param name="message">The error message to log.</param>
        public virtual void Error(string message)
        {
            Console.WriteLine($"[Error] {message}");
        }

        /// <summary>
        /// Logs a general information message to the console.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public virtual void Log(string message)
        {
            Console.WriteLine($"[Log] {message}");
        }

        /// <summary>
        /// Logs a warning message to the console.
        /// </summary>
        /// <param name="message">The warning message to log.</param>
        public virtual void Warn(string message)
        {
            Console.WriteLine($"[Warning] {message}");
        }
    }
}
