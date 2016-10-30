using System;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace StratedgemeMonitor.AspNetCore.Utils
{
    internal static class AzureADAuthenticator
    {
        public static async Task<string> RetrieveAccessToken(ClaimsPrincipal user, ISession session)
        {
            try
            {
                string userObjectID = (user.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier"))?.Value;
                AuthenticationContext authContext = new AuthenticationContext(Startup.Authority, new NaiveSessionCache(userObjectID, session));
                ClientCredential credential = new ClientCredential(Startup.ClientId, Startup.ClientSecret);

                return (await authContext.AcquireTokenAsync(Startup.MonitorBackendServerResourceId, credential))?.AccessToken;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
