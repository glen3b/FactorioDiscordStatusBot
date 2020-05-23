using System;
using System.Collections.Generic;
using System.Text;

namespace FactorioDiscordStatusBot.Models
{
    public readonly struct PlayerListEntry
    {
        public string Name { get; }
        public bool IsOnline { get; }

        public PlayerListEntry(string name, bool isOnline)
        {
            Name = name ?? throw new ArgumentException(nameof(name));
            IsOnline = isOnline;
        }
    }
}
