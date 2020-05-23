using Discord;
using Discord.Commands;
using Discord.WebSocket;
using FactorioDiscordStatusBot.ServiceImplementation;
using FactorioDiscordStatusBot.Services;
using FactorioDiscordStatusBot.Services.GameInfoProvider;
using FactorioDiscordStatusBot.Services.ServerConnection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FactorioDiscordStatusBot
{
    class Program
    {
        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            // You should dispose a service provider created using ASP.NET
            // when you are finished using it, at the end of your app's lifetime.
            // If you use another dependency injection framework, you should inspect
            // its documentation for the best way to do this.
            using (var services = ConfigureServices())
            {
                var client = services.GetRequiredService<DiscordSocketClient>();

                client.Log += LogAsync;
                services.GetRequiredService<CommandService>().Log += LogAsync;

                // Tokens should be considered secret data and never hard-coded.
                // We can read from the environment variable to avoid hardcoding.
                await client.LoginAsync(TokenType.Bot, services.GetRequiredService<IConfiguration>().GetSection("discord")["token"]);
                await client.StartAsync();

                // Here we initialize the logic required to register our commands.
                await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

                await Task.Delay(Timeout.Infinite);
            }
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());

            return Task.CompletedTask;
        }

        private ServiceProvider ConfigureServices()
        {
            var confBuild = new ConfigurationBuilder();
            confBuild.SetBasePath(Directory.GetCurrentDirectory());
            confBuild.AddJsonFile("bot.json");

            var conf = confBuild.Build();

            var servBuilder = new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<IConfiguration>(conf)
                .AddScoped<IRconProvider, SshRconProvider>()
                .AddScoped<IPlayerListProvider, RconPlayerListProvider>();

            servBuilder.Configure<SshRconProvider.Configuration>(conf.GetSection("sshRcon"));

            return servBuilder.BuildServiceProvider();
        }
    }
}
