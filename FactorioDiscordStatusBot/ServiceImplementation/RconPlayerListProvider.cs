using CoreRCON;
using FactorioDiscordStatusBot.Models;
using FactorioDiscordStatusBot.Services.GameInfoProvider;
using FactorioDiscordStatusBot.Services.ServerConnection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FactorioDiscordStatusBot.ServiceImplementation
{
    public class RconPlayerListProvider : IPlayerListProvider
    {
        private const string OnlinePlayerIndicationString = " (online)";

        private IRconProvider rconProvider;

        public RconPlayerListProvider(IRconProvider provider)
        {
            rconProvider = provider;
        }

        public async IAsyncEnumerable<PlayerListEntry> GetPlayerList()
        {
            RCON rcon = await rconProvider.GetRconConnection();
            string playerList = await rcon.SendCommandAsync("/players");
            
            foreach (string playerEntry in playerList.Split("\r\n").Skip(1))
            {
                string rawPlayer = playerEntry.Trim();
                bool isOnline = rawPlayer.EndsWith(OnlinePlayerIndicationString);
                if (isOnline) rawPlayer = rawPlayer.Substring(0, rawPlayer.Length - OnlinePlayerIndicationString.Length);
                yield return new PlayerListEntry(rawPlayer, isOnline);
            }
        }
    }
}
