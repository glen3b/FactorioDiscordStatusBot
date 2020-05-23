using CoreRCON;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FactorioDiscordStatusBot.Services.ServerConnection
{
    public interface IRconProvider
    {
        /// <summary>
        /// Obtains an RCON connection instance to the game server.
        /// </summary>
        /// <returns>A task which resolves to an RCON connection.</returns>
        Task<RCON> GetRconConnection();
    }
}
