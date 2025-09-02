using Bank_Configuration_Portal.BLL.Interfaces;
using Bank_Configuration_Portal.Common;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

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
        [HttpGet, Route("")]
        public async Task<IHttpActionResult> GetBranches(bool includeInactive = false)
        {
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
                WindowsEventLogger.WriteError($"[BranchesController.GetBranches] {ex}");
                return InternalServerError();
            }
        }
    }
}
