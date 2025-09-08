using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Configuration_Portal.Common.Auth
{
    public interface ITokenStore
    {
        string IssueAccessToken(string userName, int bankId, TimeSpan ttl, IDictionary<string, string> claims = null);
        string IssueRefreshToken(string userName, int bankId, TimeSpan ttl);
        bool TryValidateAccessToken(string token, out TokenPrincipal principal);
        bool TryValidateRefreshToken(string token, out RefreshRecord principal);
        void RevokeAccess(string token);
        void RevokeRefresh(string token);
        void RevokeAllForUser(string userName, int bankId);
        bool TryRedeemRefreshToken(string token, out TokenPrincipal principal, out bool reuseDetected);
    }
}
