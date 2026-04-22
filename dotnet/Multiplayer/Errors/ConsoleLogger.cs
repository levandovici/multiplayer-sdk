using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Errors
{
    public class ConsoleLogger : ILogger
    {
        public virtual void Error(string message)
        {
            Console.WriteLine($"[Error] {message}");
        }

        public virtual void Log(string message)
        {
            Console.WriteLine($"[Log] {message}");
        }

        public virtual void Warn(string message)
        {
            Console.WriteLine($"[Warning] {message}");
        }
    }
}
