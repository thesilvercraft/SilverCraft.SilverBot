using Lavalink4NET.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SilverBotDS.Objects.Classes
{
    public struct SerialisableQueue
    {
        public string[] Identifiers { get; set; }

        public double CurrentSongTimems { get; set; }
    }
}