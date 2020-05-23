using System;
using System.Collections.Generic;
using System.Text;

namespace FactorioDiscordStatusBot.Models
{
    public class SshAuthenticationConfig
    {
        public string Username { get; set; }
        public AuthenticationType AuthenticationMethod { get; set; }
        public string AuthenticationSecret { get; set; }

        // TODO validate username
        public SshAuthenticationConfig() { }

        public SshAuthenticationConfig(string username, AuthenticationType method, string secret)
        {
            Username = username ?? throw new ArgumentNullException(nameof(username));
            AuthenticationMethod = method;
            AuthenticationSecret = secret;
        }

        public enum AuthenticationType
        {
            Password,
            PrivateKeyFile
        }
    }
}
