using Bank_Configuration_Portal.BLL.Interfaces;
using Bank_Configuration_Portal.Common;
using Bank_Configuration_Portal.DAL.DAL;
using Bank_Configuration_Portal.Models.Api;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Bank_Configuration_Portal.Api.Controllers
{
    [Authorize]
    [RoutePrefix("api/branches")]
    public class BranchesController : ApiController
    {
        private readonly IBranchManager _branchManager;

        public BranchesController(IBranchManager branchManager)
        {
            _branchManager = branchManager;
        }

        // GET /api/branches?includeInactive=false
        /// <summary>Get branches for the authenticated bank.</summary>
        /// <param name="includeInactive">
        /// When <c>true</c>, include inactive branches; default is <c>false</c>.
        /// </param>
        /// <response code="200">Branches returned.</response>
        /// <response code="401">Missing/invalid token or bank context.</response>
        /// <response code="404">No branches found for this bank.</response>
        /// <response code="500">Server error.</response>
        [HttpGet, Route("")]
        [SwaggerOperation(OperationId = "Branches_Get", Tags = new[] { "Branches" })]
        [ResponseType(typeof(List<BranchApiModel>))]
        [SwaggerResponse(200, "Branches for the authenticated bank", typeof(List<BranchApiModel>))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "No branches found")]
        [SwaggerResponse(500, "Server error")]
        public async Task<IHttpActionResult> GetBranches(bool includeInactive = false)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid query parameter. 'includeInactive' must be (true|false).");
            try
            {
                var bankIdStr = (User as ClaimsPrincipal)?.FindFirst("BankId")?.Value;
                if (string.IsNullOrEmpty(bankIdStr) || !int.TryParse(bankIdStr, out var bankId))
                    return Unauthorized();

                var items = await _branchManager.GetAllForApiByBankIdAsync(bankId, includeInactive);
                if (items == null || items.Count == 0) return NotFound();

                return Ok(items);
            }
            catch (Exception ex)
            {
                WindowsEventLogger.WriteError(ex, "[BranchesController.GetBranches]");
                return InternalServerError();
            }
        }
    }
}
