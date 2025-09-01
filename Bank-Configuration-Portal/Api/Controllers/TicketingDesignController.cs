using Bank_Configuration_Portal.BLL.Api;
using Bank_Configuration_Portal.Common;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace Bank_Configuration_Portal.Api.Controllers
{
    [Authorize]
    [RoutePrefix("api/branches")]
    public class TicketingDesignController : ApiController
    {
        private readonly ITicketingDesignManager _screenManager;
        public TicketingDesignController(ITicketingDesignManager screenManager)
        {
            _screenManager = screenManager;
        }

        // GET /api/branches/{branchId}/screen-design?onlyAllocated=true
        [HttpGet, Route("{branchId:int}/screen-design")]
        public async Task<IHttpActionResult> GetActiveDesign(int branchId, bool onlyAllocated = true)
        {
            if (branchId <= 0) return BadRequest("Invalid branchId.");

            try
            {
                var bankIdStr = (User as ClaimsPrincipal)?.FindFirst("BankId")?.Value;
                if (!int.TryParse(bankIdStr, out var bankId)) return Unauthorized();

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
