using Bank_Configuration_Portal.BLL.Api;
using Bank_Configuration_Portal.BLL.Interfaces; 
using Bank_Configuration_Portal.Common;
using Bank_Configuration_Portal.Models.Api;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

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
        /// <summary>Get the active Ticketing Screen Design and its buttons.</summary>
        /// <param name="branchId">
        /// If omitted: return only <c>ShowMessage</c> buttons.
        /// If provided: include <c>IssueTicket</c> buttons; when <c>onlyAllocated=true</c>,
        /// those must be allocated to ≥1 active counter in that branch.
        /// </param>
        /// <param name="onlyAllocated">
        /// When <c>true</c>, requires <c>branchId</c>. Filters <c>IssueTicket</c> buttons to allocated services.
        /// Default is <c>false</c>.
        /// </param>
        /// <response code="200">Active screen returned.</response>
        /// <response code="400">Bad query parameters.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">No active screen or no buttons matching filters.</response>
        /// <response code="504">Database timeout.</response>
        [HttpGet, Route("screen-design")]
        [SwaggerOperation(OperationId = "ScreenDesign_Get", Tags = new[] { "ScreenDesign" })]
        [ResponseType(typeof(TicketingDesignModel))]
        [SwaggerResponse(200, "Active screen", typeof(TicketingDesignModel))]
        [SwaggerResponse(400, "Bad request")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "Not found")]
        [SwaggerResponse(504, "Database timeout")]
        [SwaggerResponse(500, "Server error")]
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
