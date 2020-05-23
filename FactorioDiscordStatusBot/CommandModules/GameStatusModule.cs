using Discord.Commands;
using FactorioDiscordStatusBot.Services.GameInfoProvider;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FactorioDiscordStatusBot.CommandModules
{
    public class GameStatusModule : ModuleBase<SocketCommandContext>
    {
        public IPlayerListProvider PlayerListProvider { get; set; }

        [Command("players")]
        public async Task GetServerPlayersAsync()
        {
            int numTotal = 0;
            int numOnline = 0;

            var resultBuilder = new StringBuilder();

            await foreach (var player in PlayerListProvider.GetPlayerList())
            {
                numTotal++;
                if (player.IsOnline) numOnline++;

                // green vs red circle
                resultBuilder.Append(player.IsOnline ? "🟢 " : "🔴 ");
                resultBuilder.AppendLine(player.Name);
            }

            if (numTotal == 0)
            {
                resultBuilder.AppendLine("This server has no players.");
            }

            await ReplyAsync($"**__Players:__**\n**{numOnline} / {numTotal} online**\n\n{resultBuilder}");
        }
    }
}
