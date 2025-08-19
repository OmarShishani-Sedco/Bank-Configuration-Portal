using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Runtime.Caching;
using System.Security.Claims;
using System.Web;
using System.Web.Caching;

[assembly: OwinStartup(typeof(Bank_Configuration_Portal.Startup))]
namespace Bank_Configuration_Portal
{
    public class Startup
    {
        private static readonly MemoryCache StampCache = MemoryCache.Default;
        private const string CookieAuthType = "AppCookie";
        private const string StampClaimType = "SessionStamp";
        private const string UaClaimType = "UA";
        private const string IpClaimType = "IP";

        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = CookieAuthType,
                LoginPath = new PathString("/Login/Index"),
                LogoutPath = new PathString("/Login/Logout"),
                CookieHttpOnly = true,
                CookieSecure = CookieSecureOption.SameAsRequest,
                SlidingExpiration = true,
                ExpireTimeSpan = TimeSpan.FromMinutes(30),

                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = async ctx =>
                    {
                        var id = ctx.Identity;
                        if (id == null || !id.IsAuthenticated)
                        {
                            ctx.RejectIdentity();
                            return;
                        }

                        var user = id.Name ?? "";
                        var bank = id.FindFirst("BankId")?.Value ?? "";
                        var stamp = id.FindFirst(StampClaimType)?.Value ?? "";

                        string cacheKey = $"stamp::{user}::{bank}";
                        var serverStamp = StampCache.Get(cacheKey) as string;
                        if (string.IsNullOrEmpty(serverStamp) || !ConstantTimeEquals(stamp, serverStamp))
                        {
                            ctx.RejectIdentity();
                            ctx.OwinContext.Authentication.SignOut(CookieAuthType);
                            return;
                        }

                        var boundUa = id.FindFirst(UaClaimType)?.Value ?? "";
                        var boundIp = id.FindFirst(IpClaimType)?.Value ?? "";

                        var currentUa = (HttpContext.Current?.Request?.UserAgent) ?? "";
                        var currentIp = GetClientIp(HttpContext.Current);

                        if (!string.Equals(boundUa, currentUa, StringComparison.Ordinal))
                        {
                            ctx.RejectIdentity();
                            ctx.OwinContext.Authentication.SignOut(CookieAuthType);
                            return;
                        }

                        if (!string.IsNullOrEmpty(boundIp) && !string.Equals(boundIp, currentIp, StringComparison.Ordinal))
                        {
                            ctx.RejectIdentity();
                            ctx.OwinContext.Authentication.SignOut(CookieAuthType);
                            return;
                        }

                        await System.Threading.Tasks.Task.CompletedTask;
                    }
                }
            });
        }

        private static string GetClientIp(HttpContext ctx)
        {
            var ip = ctx?.Request?.UserHostAddress;
            return string.IsNullOrEmpty(ip) ? "" : ip;
        }

        private static bool ConstantTimeEquals(string a, string b)
        {
            if (a == null || b == null || a.Length != b.Length) return false;
            int diff = 0;
            for (int i = 0; i < a.Length; i++) diff |= a[i] ^ b[i];
            return diff == 0;
        }

     
        public static string GetOrIssueNewStamp(string user, string bankId)
        {
            var key = $"stamp::{user.ToLower()}::{bankId}";
            var existing = StampCache.Get(key) as string;
            if (!string.IsNullOrEmpty(existing))
            {
                return existing;
            }

            var stamp = Guid.NewGuid().ToString("N");
            StampCache.Set(key, stamp, new CacheItemPolicy());
            return stamp;
        }

        public static void RevokeStamp(string user, string bankId)
        {
            var key = $"stamp::{user.ToLower()}::{bankId}";
            StampCache.Remove(key);
        }

    }
}
