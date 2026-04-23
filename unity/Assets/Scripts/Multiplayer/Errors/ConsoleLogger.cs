using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Michitai.Multiplayer.Errors
{
    public class ConsoleLogger : ILogger
    {
        public void Error(string message) => Debug.LogError($"[SDK Error] {message}");
        public void Log(string message) => Debug.Log($"[SDK] {message}");
        public void Warn(string message) => Debug.LogWarning($"[SDK Warning] {message}");
    }
}
