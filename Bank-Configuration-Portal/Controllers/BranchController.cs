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
        public ActionResult Index(int page = 1, int pageSize = 6)
        {
            try
            {
                int bankId = (int)Session["BankId"];
                var allBranches = _branchManager.GetAllByBankId(bankId);

                int totalBranches = allBranches.Count;
                var pagedBranches = allBranches
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var vmList = _mapper.Map<List<BranchViewModel>>(pagedBranches);

                var viewModel = new BranchListViewModel
                {
                    Branches = vmList,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalCount = totalBranches
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
            return View("CreateOrEdit",new BranchViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BranchViewModel model)
        {
            if (!ModelState.IsValid)
                return View("CreateOrEdit", model);

            try
            {
                // Map from viewmodel to model
                var branchModel = _mapper.Map<BranchModel>(model);
                branchModel.BankId = (int)Session["BankId"];
                _branchManager.Create(branchModel);
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
        public ActionResult Edit(int id)
        {
            try
            {
                int bankId = (int)Session["BankId"];
                var branch = _branchManager.GetById(id, bankId);
                if (branch == null)
                {
                    TempData["Error"] = Language.Branch_Not_Found;
                    return RedirectToAction("Index");
                }

                var vm = _mapper.Map<BranchViewModel>(branch); 
                return View("CreateOrEdit",vm);
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
        public ActionResult Edit(BranchViewModel model, bool forceUpdate = false)
        {
            if (!ModelState.IsValid)
                return View("CreateOrEdit", model);

            try
            {
                var branchModel = _mapper.Map<BranchModel>(model);
                branchModel.BankId = (int)Session["BankId"];
                var existingBranch = _branchManager.GetById(model.Id, branchModel.BankId);
                if (existingBranch != null && !forceUpdate)
                {
                    if (UiUtility.AreObjectsEqual(existingBranch, branchModel, "RowVersion", "Id", "BankId"))
                    {
                        TempData["Info"] = Language.Branch_NoChangesDetected;
                        return RedirectToAction("Index");
                    }
                }
                _branchManager.Update(branchModel, forceUpdate);

                TempData["Success"] = Language.Branch_Updated_Successfully;
                return RedirectToAction("Index");
            }
            catch (DBConcurrencyException)
            {
                if (!forceUpdate)
                {
                    // Load the latest RowVersion from DB to update model
                    var latestBranch = _branchManager.GetById(model.Id, (int)Session["BankId"]);
                    if (latestBranch != null)
                        model.RowVersion = latestBranch.RowVersion;

                    ModelState.AddModelError("", Language.Branch_Concurrency_Error + " " + Language.Branch_Concurrency_ForcePrompt);
                    ViewBag.ShowForceUpdate = true;
                }
                else
                {
                    ModelState.AddModelError("", Language.Branch_Concurrency_ForceFailed);
                }
                return View("CreateOrEdit", model);
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, byte[] rowVersion)
        {
            try
            {
                int bankId = (int)Session["BankId"];
                _branchManager.Delete(id, bankId, rowVersion);
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
