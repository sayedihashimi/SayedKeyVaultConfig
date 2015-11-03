using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.Configuration;
using Microsoft.Azure.KeyVault;

namespace SayedKeyVaultConfig {
    public class KeyVaultConfigurationProvider : ConfigurationProvider {

        private string KeyVaultUri { get; }
        private string ClientId { get; }
        private string ClientSecret { get; }
        private KeyVaultClient VaultClient { get; }


        public KeyVaultConfigurationProvider(string keyVaultUri, string clientId, string clientSecret) {
            if (string.IsNullOrEmpty(keyVaultUri)) { throw new ArgumentNullException(nameof(keyVaultUri)); }

            if (string.IsNullOrWhiteSpace(clientId)) {
                clientId = Environment.GetEnvironmentVariable("ClientId");
            }
            if (string.IsNullOrWhiteSpace(clientSecret)) {
                clientSecret = Environment.GetEnvironmentVariable("ClientSecret");
            }


            if (string.IsNullOrWhiteSpace("clientId")) {
                throw new InvalidOperationException("Did not find required env var ClientId");
            }
            if (string.IsNullOrWhiteSpace(clientSecret)) {
                throw new InvalidOperationException("Did not find required env var ClientSecret");
            }

            KeyVaultUri = keyVaultUri;
            ClientId = clientId;
            ClientSecret = clientSecret;
            VaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(KeyUtils.GetToken));
        }

        public override bool TryGet(string key, out string value) {
            var result = VaultClient.GetSecretAsync(KeyVaultUri, key, null).Result.Value;

            if (!string.IsNullOrWhiteSpace(result)) {
                value = result;
                return true;
            }
            else {
                value = null;
                return false;
            }
        }
    }

    public static class KeyVaultConfigurationExtensions {
        public static IConfigurationBuilder AddKeyVault(this IConfigurationBuilder configurationBuilder, string keyVaultUri, string clientId, string clientSecret) {
            if (configurationBuilder == null) { throw new ArgumentNullException(nameof(configurationBuilder)); }
            if (string.IsNullOrEmpty(keyVaultUri)) { throw new ArgumentNullException(nameof(keyVaultUri)); }

            configurationBuilder.Add(new KeyVaultConfigurationProvider(keyVaultUri, clientId, clientSecret));

            return configurationBuilder;
        }
    }
}
