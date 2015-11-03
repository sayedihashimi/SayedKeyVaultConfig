using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace SayedKeyVaultConfig
{
    public class KeyUtils {
        //this is an optional property to hold the secret after it is retrieved
        public static string EncryptSecret { get; set; }

        //the method that will be provided to the KeyVaultClient
        public async static Task<string> GetToken(string authority, string resource, string scope) {
            // TODO: Where to get these from?
            string clientid = Environment.GetEnvironmentVariable("ClientId");
            string clientsecret = Environment.GetEnvironmentVariable("ClientSecret");

            if (string.IsNullOrWhiteSpace(clientid)) {
                throw new InvalidOperationException("Did not find required env var ClientId");
            }
            if (string.IsNullOrWhiteSpace(clientsecret)) {
                throw new InvalidOperationException("Did not find required env var ClientSecret");
            }
            var authContext = new AuthenticationContext(authority);
            ClientCredential clientCred = new ClientCredential(clientid, clientsecret);
            AuthenticationResult result = await authContext.AcquireTokenAsync(resource, clientCred);

            if (result == null)
                throw new InvalidOperationException("Failed to obtain the JWT token");

            return result.AccessToken;
        }
    }
}
