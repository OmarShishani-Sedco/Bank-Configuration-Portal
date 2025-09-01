// Api/Auth/ApiTokenHandler.cs
using Bank_Configuration_Portal.Common.Auth;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Bank_Configuration_Portal.Api.Auth
{
    public sealed class ApiTokenHandler : DelegatingHandler
    {
        public static ITokenStore TokenStore { get; set; }

        private static string ReadAccessToken(HttpRequestMessage request)
        {
            if (request.Headers.Authorization != null &&
                request.Headers.Authorization.Scheme.Equals("Bearer", StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrWhiteSpace(request.Headers.Authorization.Parameter))
                return request.Headers.Authorization.Parameter;

            return null;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = ReadAccessToken(request);
            if (!string.IsNullOrEmpty(token) && TokenStore != null &&
                TokenStore.TryValidateAccessToken(token, out var principal))
            {
                var identity = new ClaimsIdentity("ApiBearer");
                identity.AddClaim(new Claim(ClaimTypes.Name, principal.UserName));
                identity.AddClaim(new Claim("BankId", principal.BankId.ToString()));
                if (principal.Claims != null)
                {
                    foreach (var kv in principal.Claims)
                        identity.AddClaim(new Claim(kv.Key, kv.Value));
                }

                var cp = new ClaimsPrincipal(identity);
                Thread.CurrentPrincipal = cp;
                if (HttpContext.Current != null)
                    HttpContext.Current.User = cp;
            }
            return base.SendAsync(request, cancellationToken);
        }
    }
}
