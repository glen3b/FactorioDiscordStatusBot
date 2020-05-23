using FactorioDiscordStatusBot.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FactorioDiscordStatusBot.Services.GameInfoProvider
{
    public interface IPlayerListProvider
    {
        IAsyncEnumerable<PlayerListEntry> GetPlayerList();
    }
}
