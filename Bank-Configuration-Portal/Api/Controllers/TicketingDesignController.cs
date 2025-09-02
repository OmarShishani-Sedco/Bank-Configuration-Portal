using Bank_Configuration_Portal.BLL.Api;
using Bank_Configuration_Portal.Common;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace Bank_Configuration_Portal.Api.Controllers
{
    [Authorize]
    [RoutePrefix("api")]
    public class TicketingDesignController : ApiController
    {
        private readonly ITicketingDesignManager _screenManager;
        public TicketingDesignController(ITicketingDesignManager screenManager)
        {
            _screenManager = screenManager;
        }

        // GET /api/screen-design?branchId={ID}&onlyAllocated={true|false}
        [HttpGet, Route("screen-design")]
        public async Task<IHttpActionResult> GetActiveDesign(int? branchId = null, bool onlyAllocated = false)
        {
            try
            {
                var bankIdStr = (User as ClaimsPrincipal)?.FindFirst("BankId")?.Value;
                if (!int.TryParse(bankIdStr, out var bankId)) return Unauthorized();

                if (branchId.HasValue && branchId.Value <= 0)
                    return BadRequest("Invalid branchId. Must be a positive integer.");

                if (onlyAllocated && !branchId.HasValue)
                    return BadRequest("branchId is required when onlyAllocated=true.");

                var screen = await _screenManager.GetActiveScreenButtonsForBranchAsync(bankId, branchId, onlyAllocated);
                if (screen == null || screen.Buttons?.Count == 0) return NotFound();
                return Ok(screen);
            }
            catch (Exception ex)
            {
                WindowsEventLogger.WriteError($"[TicketingDesignController.GetActiveDesign] {ex}");
                return InternalServerError();
            }
        }
    }
}
