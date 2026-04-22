using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Updates
{
    public class PollUpdatesResponse<T> : ApiResponse<ERoomUpdatesPollError> where T : class, new()
    {
        public List<PlayerUpdate<T>> Updates { get; set; } = new();
        public string Last_update { get; set; } = string.Empty;
    }
}
