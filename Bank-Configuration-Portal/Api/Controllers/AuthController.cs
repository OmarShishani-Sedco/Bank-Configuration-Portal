using Bank_Configuration_Portal.BLL.Interfaces;
using Bank_Configuration_Portal.Common;
using Bank_Configuration_Portal.Common.Auth;
using Bank_Configuration_Portal.Models.Api;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Bank_Configuration_Portal.Api.Controllers
{
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        private readonly IUserManager _userManager;
        private readonly IBankManager _bankManager;
        private readonly ITokenStore _tokens;
        private static readonly TimeSpan AccessTtl = TimeSpan.FromMinutes(20);
        private static readonly TimeSpan RefreshTtl = TimeSpan.FromHours(24);

        public AuthController(IUserManager userManager, ITokenStore tokens, IBankManager bankManager)
        {
            _userManager = userManager;
            _tokens = tokens;
            _bankManager = bankManager;
        }


        [AllowAnonymous]
        [HttpPost, Route("token")]
        public async Task<IHttpActionResult> IssueTokens(TokenRequestModel req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.UserName) ||
                string.IsNullOrWhiteSpace(req.Password) || string.IsNullOrWhiteSpace(req.BankName))
                return BadRequest("Missing credentials.");

            try
            {
                var bank = await _bankManager.GetByNameAsync(req.BankName);
                if (bank == null)
                    return BadRequest("Invalid bank Name.");

                var (valid, mustChange) = await _userManager.ValidateCredentialsAsync(req.UserName, req.Password);
                if (!valid || mustChange)
                    return Unauthorized();

                if (!await _bankManager.IsUserMappedToBankAsync(req.UserName, bank.Id))
                    return Unauthorized();

                _tokens.RevokeAllForUser(req.UserName, bank.Id);

                var access = _tokens.IssueAccessToken(req.UserName, bank.Id, AccessTtl);
                var refresh = _tokens.IssueRefreshToken(req.UserName, bank.Id, RefreshTtl);

                return Ok(new
                {
                    access_token = access,
                    expires_in = (int)AccessTtl.TotalMinutes,
                    refresh_token = refresh,
                    refresh_expires_in = (int)RefreshTtl.TotalMinutes
                });
            }
            catch (Exception ex)
            {
                WindowsEventLogger.WriteError(ex, "[AuthController.IssueTokens]");
                return InternalServerError();
            }
        }

        [AllowAnonymous]
        [HttpPost, Route("refresh")]
        public IHttpActionResult RefreshTokens(RefreshRequestModel req)
        {
            var refresh = req?.RefreshToken;
            if (string.IsNullOrWhiteSpace(refresh))
                return BadRequest("Missing refresh token.");

            try
            {
                if (!_tokens.TryRedeemRefreshToken(refresh, out var principal, out var reuseDetected))
                {
                    if (reuseDetected)
                    {
                        return Unauthorized();
                    }
                    return Unauthorized(); 
                }


                var newAccess = _tokens.IssueAccessToken(principal.UserName, principal.BankId, AccessTtl);
                var newRefresh = _tokens.IssueRefreshToken(principal.UserName, principal.BankId, RefreshTtl);

                return Ok(new
                {
                    access_token = newAccess,
                    expires_in = (int)AccessTtl.TotalMinutes,
                    refresh_token = newRefresh,
                    refresh_expires_in = (int)RefreshTtl.TotalMinutes
                });
            }
            catch (Exception ex)
            {
                WindowsEventLogger.WriteError(ex, "[AuthController.RefreshTokens]");
                return InternalServerError();
            }
        }

        [HttpPost, Route("revoke")]
        public IHttpActionResult RevokeTokens(RevokeRequestModel rev)
        {
            try
            {
                var auth = Request.Headers.Authorization;
                if (auth == null || !auth.Scheme.Equals("Bearer", StringComparison.OrdinalIgnoreCase) ||
                    string.IsNullOrWhiteSpace(auth.Parameter))
                {
                    return Unauthorized();
                }

                var accessToken = auth.Parameter;

                if (!_tokens.TryValidateAccessToken(accessToken, out var accessPrincipal))
                {
                    return Unauthorized();
                }

                _tokens.RevokeAccess(accessToken);

                var refreshToken = rev?.RefreshToken;
                if (!string.IsNullOrWhiteSpace(refreshToken))
                {
                    if (_tokens.TryValidateRefreshToken(refreshToken, out var refreshPrincipal))
                    {
                        if (string.Equals(refreshPrincipal.UserName, accessPrincipal.UserName, StringComparison.Ordinal) &&
                            refreshPrincipal.BankId == accessPrincipal.BankId)
                        {
                            _tokens.RevokeRefresh(refreshToken);
                        }
                    }
                }

                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                WindowsEventLogger.WriteError(ex, "[AuthController.RevokeTokens]");
                return InternalServerError();
            }
        }

    }
}
