using AutoMapper;
using Bank_Configuration_Portal.BLL;
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
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Bank_Configuration_Portal.Controllers
{
    public class ServiceController : BaseController
    {
        private readonly IServiceManager _serviceManager;
        private readonly IMapper _mapper;
        private const int pageSize = 6;

        public ServiceController(IServiceManager serviceManager, IMapper mapper)
        {
            _serviceManager = serviceManager;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> Index(string searchTerm, bool? isActive, int page = 1)
        {
            try
            {
                if (!TryGetBankId(out var bankId)) return BankIdMissingRedirect();

                var data = await _serviceManager.GetPagedByBankIdAsync(bankId, searchTerm, isActive, page, pageSize);

                var vmList = _mapper.Map<List<ServiceViewModel>>(data.Items);

                var viewModel = new ServiceListViewModel
                {
                    Services = vmList,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalCount = data.TotalCount,
                    SearchTerm = searchTerm,
                    IsActive = isActive
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "ServiceController.Index");
                TempData["Error"] = Language.Generic_Error;
                return View(new ServiceListViewModel());
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View("CreateOrEdit", new ServiceViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ServiceViewModel model)
        {
            if (!ModelState.IsValid)
                return View("CreateOrEdit", model);

            try
            {
                var serviceModel = _mapper.Map<ServiceModel>(model);
                if (!TryGetBankId(out var bankId)) return BankIdMissingRedirect();
                serviceModel.BankId = bankId;
                await _serviceManager.CreateAsync(serviceModel);

                TempData["Success"] = Language.Service_Created_Successfully;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "ServiceController.Create(POST)");
                ModelState.AddModelError("", Language.Generic_Error);
                return View("CreateOrEdit", model);
            }
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            try
            {
                var service = await _serviceManager.GetByIdAsync(id);
                if (service == null)
                {
                    TempData["Error"] = Language.Service_Not_Found;
                    return RedirectToAction("Index");
                }

                var vm = _mapper.Map<ServiceViewModel>(service);
                return View("CreateOrEdit", vm);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "ServiceController.Edit(GET)");
                TempData["Error"] = Language.Generic_Error;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ServiceViewModel model, bool forceUpdate = false)
        {
            if (!ModelState.IsValid)
                return View("CreateOrEdit", model);

            try
            {
                var serviceModel = _mapper.Map<ServiceModel>(model);
                if (!TryGetBankId(out var bankId)) return BankIdMissingRedirect();
                serviceModel.BankId = bankId;
                var existingService = await _serviceManager.GetByIdAsync(model.Id);
                if (existingService == null)
                    {
                    TempData["Error"] = Language.Service_Not_Found + " " + Language.Service_Already_Deleted;
                    return RedirectToAction("Index");
                }

               var isChanged = await _serviceManager.UpdateAsync(serviceModel, existingService, forceUpdate);
                if (!isChanged)
                {
                    TempData["Info"] = Language.Service_NoChangesDetected;
                    return RedirectToAction("Index");
                }

                TempData["Success"] = Language.Service_Updated_Successfully;
                return RedirectToAction("Index");
            }
            catch (DBConcurrencyException)
            {
                if (!forceUpdate)
                {
                    ModelState.AddModelError("", Language.Service_Concurrency_Error + " " + Language.Concurrency_ForcePrompt);
                    ViewBag.ConflictMode = true;
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
        public async Task<ActionResult> Delete(int id, byte[] rowVersion, bool forceDelete = false)
        {
            try
            {
                await _serviceManager.DeleteAsync(id, rowVersion, forceDelete);
                TempData["Success"] = Language.Service_Deleted_Successfully;
            }
            catch (CustomConcurrencyDeletedException)
            {
                TempData["Error"] = Language.Service_Already_Deleted;
            }
            catch (CustomConcurrencyModifiedException)
            {
                if (!forceDelete)
                {
                    TempData["ConcurrencyConflictId"] = id;
                    TempData["OriginalRowVersion"] = rowVersion;
                    TempData["Error"] = Language.Service_Delete_Concurrency_Error;
                }
                else
                {
                    TempData["Error"] = Language.Concurrency_ForceFailed;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "ServiceController.Delete(POST)");
                TempData["Error"] = Language.Generic_Error;
            }

            return RedirectToAction("Index");
        }



    }

}