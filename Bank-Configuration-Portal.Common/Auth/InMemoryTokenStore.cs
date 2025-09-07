using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Security.Cryptography;

namespace Bank_Configuration_Portal.Common.Auth
{
    public sealed class InMemoryTokenStore : ITokenStore
    {
        private static readonly MemoryCache Cache = MemoryCache.Default;

        private static string Key(string kind, string token) => $"api::{kind}::{token}";

        private static string NewToken()
        {
            try
            {
                var bytes = new byte[32];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(bytes);
                }

                return Convert.ToBase64String(bytes)
                    .TrimEnd('=')
                    .Replace('+', '-')
                    .Replace('/', '_');
            }
            catch (Exception ex)
            {
                WindowsEventLogger.WriteError(ex, "[InMemoryTokenStore.NewToken]");
                throw new InvalidOperationException("Failed to generate secure token.", ex);
            }
        }

        public string IssueAccessToken(string userName, int bankId, TimeSpan ttl, IDictionary<string, string> claims = null)
        {
            try
            {
                var token = NewToken();
                var principal = new TokenPrincipal
                {
                    UserName = userName,
                    BankId = bankId,
                    Claims = claims ?? new Dictionary<string, string>(),
                    ExpiresAt = DateTimeOffset.UtcNow.Add(ttl)
                };

                Cache.Set(Key("access", token), principal,
                    new CacheItemPolicy { AbsoluteExpiration = principal.ExpiresAt });

                return token;
            }
            catch (Exception ex)
            {
                WindowsEventLogger.WriteError(ex, "[InMemoryTokenStore.IssueAccessToken]");
                throw;
            }
        }

        public string IssueRefreshToken(string userName, int bankId, TimeSpan ttl)
        {
            try
            {
                var token = NewToken();
                var rec = new RefreshRecord
                {
                    UserName = userName,
                    BankId = bankId,
                    ExpiresAt = DateTimeOffset.UtcNow.Add(ttl)
                };
                Cache.Set(
                    Key("refresh",token),
                    rec,
                    new CacheItemPolicy { AbsoluteExpiration = rec.ExpiresAt }
                );
                return token;
            }
            catch (Exception ex)
            {
                WindowsEventLogger.WriteError(ex, "[InMemoryTokenStore.IssueRefreshToken]");
                throw;
            }

        }

        public bool TryValidateAccessToken(string token, out TokenPrincipal principal)
        {
            principal = null;
            if (string.IsNullOrWhiteSpace(token)) return false;

            try
            {
                principal = Cache.Get(Key("access", token)) as TokenPrincipal;
                return principal != null && principal.ExpiresAt > DateTimeOffset.UtcNow;
            }
            catch (Exception ex)
            {
                WindowsEventLogger.WriteError(ex, "[InMemoryTokenStore.TryValidateAccessToken]");
                principal = null;
                return false;
            }
        }

        public bool TryValidateRefreshToken(string token, out TokenPrincipal principal)
        {
            principal = null;
            if (string.IsNullOrWhiteSpace(token)) return false;

            try
            {
                principal = Cache.Get(Key("refresh", token)) as TokenPrincipal;
                return principal != null && principal.ExpiresAt > DateTimeOffset.UtcNow;
            }
            catch (Exception ex)
            {
                WindowsEventLogger.WriteError(ex, "[InMemoryTokenStore.TryValidateRefreshToken]");
                principal = null;
                return false;
            }
        }

        public bool TryRedeemRefreshToken(string token, out TokenPrincipal principal, out bool reuseDetected)
        {
            principal = null;
            reuseDetected = false;
            if (string.IsNullOrWhiteSpace(token)) return false;

            try
            {
                var key = Key("refresh", token);
                var rec = Cache.Get(key) as RefreshRecord;
                if (rec == null || rec.ExpiresAt <= DateTimeOffset.UtcNow)
                    return false;

                if (rec.UsedAt != null)
                {
                    reuseDetected = true;
                    RevokeAllForUser(rec.UserName, rec.BankId);
                    return false;
                }

                rec.UsedAt = DateTimeOffset.UtcNow;
                Cache.Set(key, rec, new CacheItemPolicy { AbsoluteExpiration = rec.ExpiresAt });

                principal = new TokenPrincipal
                {
                    UserName = rec.UserName,
                    BankId = rec.BankId,
                    ExpiresAt = rec.ExpiresAt
                };
                return true;
            }
            catch (Exception ex)
            {
                WindowsEventLogger.WriteError(ex, "[InMemoryTokenStore.TryConsumeRefreshToken]");
                principal = null;
                reuseDetected = false;
                return false;
            }
        }

        public void RevokeAccess(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return;
            try
            {
                Cache.Remove(Key("access", token));
            }
            catch (Exception ex)
            {
                WindowsEventLogger.WriteError(ex, "[InMemoryTokenStore.RevokeAccess]");
            }
        }

        public void RevokeRefresh(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return;
            try
            {
                Cache.Remove(Key("refresh", token));
            }
            catch (Exception ex)
            {
                WindowsEventLogger.WriteError(ex, "[InMemoryTokenStore.RevokeRefresh]");
            }
        }

        public void RevokeAllForUser(string userName, int bankId)
        {
            if (string.IsNullOrWhiteSpace(userName)) return;

            try
            {
                var toRemove = new List<string>();

                foreach (var kv in Cache)
                {
                    var key = kv.Key;

                    if (!(key.StartsWith("api::access::") || key.StartsWith("api::refresh::")))
                        continue;

                    if (kv.Value is TokenPrincipal ap)
                    {
                        if (ap.BankId == bankId &&
                            ap.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase))
                        {
                            toRemove.Add(key);
                        }
                    }
                    else if (kv.Value is RefreshRecord rr)
                    {
                        if (rr.BankId == bankId &&
                            rr.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase))
                        {
                            toRemove.Add(key);
                        }
                    }
                }

                foreach (var k in toRemove)
                    Cache.Remove(k);
            }
            catch (Exception ex)
            {
                WindowsEventLogger.WriteError(ex, "[InMemoryTokenStore.RevokeAllForUser]");
            }
        }
    }
}
