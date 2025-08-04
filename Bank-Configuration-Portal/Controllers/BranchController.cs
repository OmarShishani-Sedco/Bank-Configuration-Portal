using AutoMapper;
using Bank_Configuration_Portal.BLL.Interfaces;
using Bank_Configuration_Portal.Common;
using Bank_Configuration_Portal.Models;
using Bank_Configuration_Portal.Models.Models;
using Bank_Configuration_Portal.Resources;
using System;
using System.Collections.Generic;
using System.Data;
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
        public ActionResult Index()
        {
            try
            {
                int bankId = (int)Session["BankId"];
                var branches = _branchManager.GetAllByBankId(bankId);
                var vmList = _mapper.Map<List<BranchViewModel>>(branches); 
                return View(vmList);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                TempData["Error"] = Language.Generic_Error;
                return View(new List<BranchViewModel>());
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
        public ActionResult Edit(BranchViewModel model)
        {
            if (!ModelState.IsValid)
                return View("CreateOrEdit",model);

            try
            {
                var branchModel = _mapper.Map<BranchModel>(model);
                branchModel.BankId = (int)Session["BankId"];
                _branchManager.Update(branchModel);

                TempData["Success"] = Language.Branch_Updated_Successfully;
                return RedirectToAction("Index");
            }
            catch (DBConcurrencyException)
            {
                ModelState.AddModelError("", Language.Branch_Concurrency_Error);
                return View("CreateOrEdit", model);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                ModelState.AddModelError("", Language.Generic_Error);
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
