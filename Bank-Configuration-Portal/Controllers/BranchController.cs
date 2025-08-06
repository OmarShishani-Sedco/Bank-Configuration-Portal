using AutoMapper;
using Bank_Configuration_Portal.BLL.Interfaces;
using Bank_Configuration_Portal.Common;
using Bank_Configuration_Portal.Models;
using Bank_Configuration_Portal.Models.Models;
using Bank_Configuration_Portal.Resources;
using Bank_Configuration_Portal.UiHelpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Bank_Configuration_Portal.Controllers
{
    public class BranchController : BaseController
    {
        private readonly IBranchManager _branchManager;
        private readonly IMapper _mapper;

        public BranchController(IBranchManager branchManager, IMapper mapper)
        {
            _branchManager = branchManager;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> Index(string searchTerm, bool? isActive, int page = 1, int pageSize = 6)
        {
            try
            {
                int bankId = (int)Session["BankId"];
                var allBranches = await _branchManager.GetAllByBankIdAsync(bankId);

                var filteredBranches = allBranches.AsQueryable();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    filteredBranches = filteredBranches.Where(b =>
                        b.NameEnglish.ToLower().Contains(searchTerm.ToLower()) ||
                        b.NameArabic.ToLower().Contains(searchTerm.ToLower()));
                }

                if (isActive.HasValue)
                {
                    filteredBranches = filteredBranches.Where(b => b.IsActive == isActive.Value);
                }

                int totalCount = filteredBranches.Count();
                var pagedBranches = filteredBranches
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var vmList = _mapper.Map<List<BranchViewModel>>(pagedBranches);

                var viewModel = new BranchListViewModel
                {
                    Branches = vmList,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    SearchTerm = searchTerm,
                    IsActive = isActive
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                TempData["Error"] = Language.Generic_Error;
                return View(new BranchListViewModel());
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View("CreateOrEdit", new BranchViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(BranchViewModel model)
        {
            if (!ModelState.IsValid)
                return View("CreateOrEdit", model);

            try
            {
                var branchModel = _mapper.Map<BranchModel>(model);
                branchModel.BankId = (int)Session["BankId"];
                await _branchManager.CreateAsync(branchModel);

                TempData["Success"] = Language.Branch_Created_Successfully;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                ModelState.AddModelError("", Language.Generic_Error);
                return View("CreateOrEdit", model);
            }
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            try
            {
                int bankId = (int)Session["BankId"];
                var branch = await _branchManager.GetByIdAsync(id, bankId);
                if (branch == null)
                {
                    TempData["Error"] = Language.Branch_Not_Found;
                    return RedirectToAction("Index");
                }

                var vm = _mapper.Map<BranchViewModel>(branch);
                return View("CreateOrEdit", vm);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                TempData["Error"] = Language.Generic_Error;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(BranchViewModel model, bool forceUpdate = false)
        {
            if (!ModelState.IsValid)
                return View("CreateOrEdit", model);

            try
            {
                var branchModel = _mapper.Map<BranchModel>(model);
                branchModel.BankId = (int)Session["BankId"];
                var existingBranch = await _branchManager.GetByIdAsync(model.Id, branchModel.BankId);

                if (existingBranch != null && !forceUpdate)
                {
                    if (UiUtility.AreObjectsEqual(existingBranch, branchModel, "RowVersion", "Id", "BankId"))
                    {
                        TempData["Info"] = Language.Branch_NoChangesDetected;
                        return RedirectToAction("Index");
                    }
                }

                await _branchManager.UpdateAsync(branchModel, forceUpdate);

                TempData["Success"] = Language.Branch_Updated_Successfully;
                return RedirectToAction("Index");
            }
            catch (DBConcurrencyException)
            {
                if (!forceUpdate)
                {
                    var latestBranch = await _branchManager.GetByIdAsync(model.Id, (int)Session["BankId"]);
                    if (latestBranch != null)
                        model.RowVersion = latestBranch.RowVersion;

                    ModelState.AddModelError("", Language.Branch_Concurrency_Error + " " + Language.Concurrency_ForcePrompt);
                    ViewBag.ShowForceUpdate = true;
                }
                else
                {
                    ModelState.AddModelError("", Language.Concurrency_ForceFailed);
                }

                return View("CreateOrEdit", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, byte[] rowVersion)
        {
            try
            {
                int bankId = (int)Session["BankId"];
                await _branchManager.DeleteAsync(id, bankId, rowVersion);
                TempData["Success"] = Language.Branch_Deleted_Successfully;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                TempData["Error"] = Language.Generic_Error;
            }

            return RedirectToAction("Index");
        }
    }
}
