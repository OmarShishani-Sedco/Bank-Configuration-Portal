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
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
namespace Bank_Configuration_Portal.Controllers
{
    public class CounterController : BaseController
    {
        private readonly ICounterManager _counterManager;
        private readonly IBranchManager _branchManager;
        private readonly IServiceManager _serviceManager;
        private readonly IMapper _mapper;

        public CounterController(ICounterManager counterManager, IBranchManager branchManager, IMapper mapper, IServiceManager serviceManager)
        {
            _counterManager = counterManager;
            _branchManager = branchManager;
            _mapper = mapper;
            _serviceManager = serviceManager;
        }

        [HttpGet]
        public async Task<ActionResult> Index(int branchId, string searchTerm, bool? isActive, int page = 1, int pageSize = 6)
        {
            if (branchId == 0)
            {
                TempData["Error"] = Language.Branch_Not_Found;
                return RedirectToAction("Index", "Branch");
            }

            try
            {
                if (!TryGetBankId(out var bankId)) return BankIdMissingRedirect();
                var allCounters = await _counterManager.GetAllByBranchIdAsync(branchId);
                var branch = await _branchManager.GetByIdAsync(branchId, bankId);

                var filteredCounters = allCounters.AsQueryable();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    filteredCounters = filteredCounters.Where(b =>
                        b.NameEnglish.ToLower().Contains(searchTerm.ToLower()) ||
                        b.NameArabic.ToLower().Contains(searchTerm.ToLower()));
                }

                if (isActive.HasValue)
                {
                    filteredCounters = filteredCounters.Where(b => b.IsActive == isActive.Value);
                }

                if (branch == null)
                {
                    TempData["Error"] = Language.Branch_Not_Found;
                    return RedirectToAction("Index", "Branch");
                }

                if (page < 1)
                {
                    return RedirectToAction("Index", new { branchId = branchId, page = 1, pageSize = pageSize });
                }

                int totalCounters = filteredCounters.Count();
                var pagedCounters = filteredCounters
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var allSelectedServiceIds = pagedCounters
                    .SelectMany(c => c.AllocatedServiceIds)
                    .Distinct()
                    .ToList();

                var allocatedServices = await _serviceManager.GetByIdsAsync(allSelectedServiceIds);
                var allocatedServiceModels = _mapper.Map<List<ServiceViewModel>>(allocatedServices);

                var vmList = _mapper.Map<List<CounterViewModel>>(pagedCounters);

                foreach (var counterViewModel in vmList)
                {
                    counterViewModel.SelectedServices = allocatedServiceModels
                        .Where(s => counterViewModel.SelectedServiceIds.Contains(s.Id))
                        .ToList();
                }

                var viewModel = new CounterListViewModel
                {
                    Counters = vmList,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalCount = totalCounters,
                    BranchId = branchId,
                    BranchNameEnglish = branch.NameEnglish,
                    BranchNameArabic = branch.NameArabic,
                    SearchTerm = searchTerm,
                    IsActive = isActive
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "CounterController.Index");
                TempData["Error"] = Language.Generic_Error;
                return View(new CounterListViewModel());
            }
        }


        [HttpGet]
        public async Task<ActionResult> Create(int branchId)
        {
            if (branchId == 0)
            {
                TempData["Error"] = Language.Branch_Not_Found;
                return RedirectToAction("Index", "Branch");
            }

            if (!TryGetBankId(out var bankId)) return BankIdMissingRedirect();
            var viewModel = new CounterViewModel
            {
                BranchId = branchId,
                AllActiveServices = _mapper.Map<List<ServiceViewModel>>(await _serviceManager.GetAllActiveByBankIdAsync(bankId))
            };
            return View("CreateOrEdit", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CounterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                if (!TryGetBankId(out var bankId)) return BankIdMissingRedirect();
                model.AllActiveServices = _mapper.Map<List<ServiceViewModel>>(await _serviceManager.GetAllActiveByBankIdAsync(bankId));
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
                Logger.LogError(ex, "CounterController.Create(POST)");
                ModelState.AddModelError("", Language.Generic_Error);
                if (!TryGetBankId(out var bankId)) return BankIdMissingRedirect();
                model.AllActiveServices = _mapper.Map<List<ServiceViewModel>>(await _serviceManager.GetAllActiveByBankIdAsync(bankId));
                return View("CreateOrEdit", model);
            }
        }



        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            try
            {
                if (!TryGetBankId(out var bankId)) return BankIdMissingRedirect();
                var counter = await _counterManager.GetByIdAsync(id);

                if (counter == null)
                {
                    TempData["Error"] = Language.Counter_Not_Found;

                    return RedirectToAction("Index", new { branchId = counter.BranchId });
                }

                var viewModel = _mapper.Map<CounterViewModel>(counter);
                viewModel.AllActiveServices = _mapper.Map<List<ServiceViewModel>>(await _serviceManager.GetAllActiveByBankIdAsync(bankId));
                viewModel.SelectedServiceIds = counter.AllocatedServiceIds ?? new List<int>();

                return View("CreateOrEdit", viewModel);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "CounterController.Edit(GET)");
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
                if (!TryGetBankId(out var bankId)) return BankIdMissingRedirect();
                model.AllActiveServices = _mapper.Map<List<ServiceViewModel>>(await _serviceManager.GetAllActiveByBankIdAsync(bankId));
                return View("CreateOrEdit", model);
            }

            try
            {
                var counter = _mapper.Map<CounterModel>(model);
                var existingCounter = await _counterManager.GetByIdAsync(model.Id);
                if (existingCounter == null)
                {
                    TempData["Error"] = Language.Counter_Not_Found + " " + Language.Counter_Already_Deleted;
                    return RedirectToAction("Index", new { branchId = counter.BranchId });
                }

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
                    if (!TryGetBankId(out var bankId)) return BankIdMissingRedirect();
                    model.AllActiveServices = _mapper.Map<List<ServiceViewModel>>(await _serviceManager.GetAllActiveByBankIdAsync(bankId));
                }
                else
                {
                    ModelState.AddModelError("", Language.Concurrency_ForceFailed);
                }
                return View("CreateOrEdit", model);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "CounterController.Edit(POST)");
                TempData["Error"] = Language.Generic_Error;
                if (!TryGetBankId(out var bankId)) return BankIdMissingRedirect();
                model.AllActiveServices = _mapper.Map<List<ServiceViewModel>>(await _serviceManager.GetAllActiveByBankIdAsync(bankId));
                return View("CreateOrEdit", model);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, byte[] rowVersion, bool forceDelete = false)
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

            await _counterManager.DeleteAsync(id, rowVersion,forceDelete); 
                TempData["Success"] = Language.Counter_Deleted_Successfully;
                return RedirectToAction("Index", new { branchId = branchId });
            }
            catch (CustomConcurrencyDeletedException)
            {
                TempData["Error"] = Language.Counter_Already_Deleted;
            }
            catch (CustomConcurrencyModifiedException)
            {
                if (!forceDelete)
                {
                    TempData["ConcurrencyConflictId"] = id;
                    TempData["OriginalRowVersion"] = rowVersion;
                    TempData["Error"] = Language.Counter_Delete_Concurrency_Error;
                }
                else
                {
                    TempData["Error"] = Language.Concurrency_ForceFailed;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "CounterController.Delete(POST)");
                TempData["Error"] = Language.Generic_Error;
                if (branchId != 0)
                {
                    return RedirectToAction("Index", new { branchId = branchId });
                }
                return RedirectToAction("Index", "Branch");
            }
            return RedirectToAction("Index", new { branchId = branchId });

        }


    }
}