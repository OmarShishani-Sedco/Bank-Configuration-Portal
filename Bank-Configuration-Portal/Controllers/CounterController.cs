using AutoMapper;
using Bank_Configuration_Portal.BLL.Interfaces;
using Bank_Configuration_Portal.Common;
using Bank_Configuration_Portal.Controllers;
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
using System.Web;

public class CounterController : BaseController
{
    private readonly ICounterManager _counterManager;
    private readonly IBranchManager _branchManager;
    private readonly IMapper _mapper;

    public CounterController(ICounterManager counterManager, IBranchManager branchManager, IMapper mapper)
    {
        _counterManager = counterManager;
        _branchManager = branchManager;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult> Index(int branchId, int page = 1, int pageSize = 6)
    {
        if (branchId == 0)
        {
            TempData["Error"] = Language.Branch_Not_Found;
            return RedirectToAction("Index", "Branch");
        }

        try
        {
            int bankId = (int)Session["BankId"];
            var allCounters = await _counterManager.GetAllByBranchIdAsync(branchId);
            var branch = await _branchManager.GetByIdAsync(branchId, bankId);

            if (branch == null)
            {
                TempData["Error"] = Language.Branch_Not_Found;
                return RedirectToAction("Index", "Branch");
            }

            if (page < 1)
            {
                return RedirectToAction("Index", new { branchId = branchId, page = 1, pageSize = pageSize });
            }

            int totalCounters = allCounters.Count;
            var pagedCounters = allCounters
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var vmList = _mapper.Map<List<CounterViewModel>>(pagedCounters);

            var viewModel = new CounterListViewModel
            {
                Counters = vmList,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCounters,
                BranchId = branchId,
                BranchNameEnglish = branch.NameEnglish,
                BranchNameArabic = branch.NameArabic
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
            TempData["Error"] = Language.Generic_Error;
            return View(new CounterListViewModel());
        }
    }

    [HttpGet]
    public ActionResult Create(int branchId)
    {
        if (branchId == 0)
        {
            TempData["Error"] = Language.Branch_Not_Found;
            return RedirectToAction("Index", "Branch");
        }

        var viewModel = new CounterViewModel { BranchId = branchId };
        return View("CreateOrEdit", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create(CounterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("CreateOrEdit", model);
        }

        try
        {
            var counter = _mapper.Map<CounterModel>(model);
            await _counterManager.CreateAsync(counter);
            TempData["Success"] = Language.Counter_Created_Successfully;
            return RedirectToAction("Index", new { branchId = counter.BranchId });
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
            var counter = await _counterManager.GetByIdAsync(id);
            if (counter == null)
            {
                TempData["Error"] = Language.Counter_Not_Found;
                return RedirectToAction("Index", "Branch");
            }

            var viewModel = _mapper.Map<CounterViewModel>(counter);
            return View("CreateOrEdit", viewModel);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
            TempData["Error"] = Language.Generic_Error;
            return RedirectToAction("Index", "Branch");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit(CounterViewModel model, bool forceUpdate = false)
    {
        if (!ModelState.IsValid)
        {
            return View("CreateOrEdit", model);
        }

        try
        {
            var counter = _mapper.Map<CounterModel>(model);
            var existingCounter = await _counterManager.GetByIdAsync(model.Id);

            if (existingCounter != null && !forceUpdate)
            {
                if (UiUtility.AreObjectsEqual(existingCounter, counter, "RowVersion", "Id", "BranchId"))
                {
                    TempData["Info"] = Language.Counter_NoChangesDetected;
                    return RedirectToAction("Index", new { branchId = counter.BranchId });
                }
            }

            await _counterManager.UpdateAsync(counter, forceUpdate);
            TempData["Success"] = Language.Counter_Updated_Successfully;
            return RedirectToAction("Index", new { branchId = counter.BranchId });
        }
        catch (DBConcurrencyException)
        {
            if (!forceUpdate)
            {
                var latestCounter = await _counterManager.GetByIdAsync(model.Id);
                if (latestCounter != null)
                {
                    model.RowVersion = latestCounter.RowVersion;
                }
                ModelState.AddModelError("", Language.Counter_Concurrency_Error + " " + Language.Concurrency_ForcePrompt);
                ViewBag.ShowForceUpdate = true;
            }
            else
            {
                ModelState.AddModelError("", Language.Concurrency_ForceFailed);
            }
            return View("CreateOrEdit", model);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
            TempData["Error"] = Language.Generic_Error;
            return View("CreateOrEdit", model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Delete(int id, byte[] rowVersion)
    {
        int branchId = 0;
        try
        {
            var counter = await _counterManager.GetByIdAsync(id);
            if (counter != null)
            {
                branchId = counter.BranchId;
            }

            if (counter == null)
            {
                TempData["Error"] = Language.Counter_Not_Found;
                return RedirectToAction("Index", "Branch");
            }

            await _counterManager.DeleteAsync(id, rowVersion);
            TempData["Success"] = Language.Counter_Deleted_Successfully;
            return RedirectToAction("Index", new { branchId = branchId });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
            TempData["Error"] = Language.Generic_Error;
            if (branchId != 0)
            {
                return RedirectToAction("Index", new { branchId = branchId });
            }
            return RedirectToAction("Index", "Branch");
        }
    }

   
}