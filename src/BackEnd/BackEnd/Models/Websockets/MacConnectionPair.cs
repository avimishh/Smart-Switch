using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fleck;

namespace BackEnd.Models.Websockets
{
    public class MacConnectionPair
    {
        public string Mac { get; set; }
        public IWebSocketConnection Socket { get; set; }
    }
}
