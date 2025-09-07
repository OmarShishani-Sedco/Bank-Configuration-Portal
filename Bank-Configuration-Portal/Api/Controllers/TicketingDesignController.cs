using Bank_Configuration_Portal.BLL.Api;
using Bank_Configuration_Portal.BLL.Interfaces; 
using Bank_Configuration_Portal.Common;
using Bank_Configuration_Portal.Models.Api;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
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
            if (!ModelState.IsValid)
                return BadRequest("Invalid query parameters. 'branchId' must be an integer and 'onlyAllocated' must be (true|false).");

            try
            {
                var bankIdStr = (User as ClaimsPrincipal)?.FindFirst("BankId")?.Value;
                if (!int.TryParse(bankIdStr, out var bankId))
                    return Unauthorized();

                if (onlyAllocated && !branchId.HasValue)
                    return BadRequest("branchId is required when onlyAllocated=true.");

                if (branchId.HasValue && branchId.Value <= 0)
                    return BadRequest("branchId must be a positive integer.");

                var (screen, status) = await _screenManager.GetActiveScreenButtonsForBranchAsync(bankId, branchId, onlyAllocated);

                switch ((ActiveScreenStatus)status)
                {
                    case ActiveScreenStatus.Ok:
                        if (screen == null || screen.Buttons == null || screen.Buttons.Count == 0)
                            return NotFound();
                        return Ok(screen);

                    case ActiveScreenStatus.InvalidBranch:
                        return NotFound(); 

                    case ActiveScreenStatus.NoActiveScreen:
                        return NotFound();

                    default:
                        return InternalServerError();
                }
            }
            catch (DatabaseTimeoutException)
            {
                return ResponseMessage(Request.CreateErrorResponse(
                    (HttpStatusCode)504, "Database timeout."));
            }
            catch (Exception ex)
            {
                WindowsEventLogger.WriteError(ex, "[TicketingDesignController.GetActiveDesign]");
                return InternalServerError();
            }
        }
    }
}
