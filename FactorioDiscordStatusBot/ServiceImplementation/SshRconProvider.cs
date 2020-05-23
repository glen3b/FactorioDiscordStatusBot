using CoreRCON;
using FactorioDiscordStatusBot.Models;
using FactorioDiscordStatusBot.Services.ServerConnection;
using Microsoft.Extensions.Options;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FactorioDiscordStatusBot.ServiceImplementation
{
    public class SshRconProvider : IRconProvider, IDisposable
    {
        private readonly Configuration providerConfig;
        private readonly Lazy<Task<SshClient>> sshClient;
        private ushort localRconPort;

        public SshRconProvider(IOptionsSnapshot<SshRconProvider.Configuration> config)
        {
            providerConfig = config.Value;
            sshClient = new Lazy<Task<SshClient>>(CreateSshClient);
        }

        public async Task<RCON> GetRconConnection()
        {
            SshClient client = await sshClient.Value;

            // TODO automagic restart
            if (!client.IsConnected) throw new InvalidOperationException("Cannot get an RCON connection; SSH has been disconnected.");
            
            RCON rcon = new RCON(IPAddress.Loopback, localRconPort, providerConfig.RconPassword);
            await rcon.ConnectAsync();
            return rcon;
        }

        public void Dispose()
        {
            if (sshClient.IsValueCreated)
            {
                SshClient client = sshClient.Value.Result;
                foreach (var port in client.ForwardedPorts)
                {
                    port.Stop();
                }
                client.Disconnect();

                client.Dispose();
            }
        }

        private async Task<SshClient> CreateSshClient()
        {
            AuthenticationMethod auth = providerConfig.SshAuthentication.AuthenticationMethod switch
            {
                SshAuthenticationConfig.AuthenticationType.Password => new PasswordAuthenticationMethod(providerConfig.SshAuthentication.Username, providerConfig.SshAuthentication.AuthenticationSecret),
                SshAuthenticationConfig.AuthenticationType.PrivateKeyFile => new PrivateKeyAuthenticationMethod(providerConfig.SshAuthentication.Username, new PrivateKeyFile(providerConfig.SshAuthentication.AuthenticationSecret)),
                _ => throw new NotSupportedException("The given authentication method is not supported."),
            };
            var client = new SshClient(new ConnectionInfo(providerConfig.SshHost, providerConfig.SshPort, providerConfig.SshAuthentication.Username, auth));
            
            // looks like there's no ConnectAsync
            await Task.Run(() => client.Connect());

            var portFwd = new ForwardedPortLocal("127.0.0.1", "127.0.0.1", providerConfig.RconPort);
            client.AddForwardedPort(portFwd);

            portFwd.Exception += (o, e) => { portFwd.Stop(); client.Disconnect(); };

            portFwd.Start();

            localRconPort = (ushort)portFwd.BoundPort;

            // TODO nicer error and disconnect handling
            return client;
        }

        public class Configuration
        {
            public string SshHost { get; set; }
            public ushort SshPort { get; set; } = 22;
            public SshAuthenticationConfig SshAuthentication { get; set; }
            public ushort RconPort { get; set; }
            public string RconPassword { get; set;  }
        }
    }
}
