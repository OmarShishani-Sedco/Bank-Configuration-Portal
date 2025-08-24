using Bank_Configuration_Portal.BLL.Interfaces;
using Bank_Configuration_Portal.Common;
using Bank_Configuration_Portal.Common.Paging;
using Bank_Configuration_Portal.DAL.DAL;
using Bank_Configuration_Portal.DAL.Interfaces;
using Bank_Configuration_Portal.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Configuration_Portal.BLL
{
    public class ServiceManager : IServiceManager
    {
        private readonly IServiceDAL _serviceDAL;

        public ServiceManager(IServiceDAL serviceDAL)
        {
            _serviceDAL = serviceDAL;
        }

        public async Task<List<ServiceModel>> GetAllByBankIdAsync(int bankId)
        {
                return await _serviceDAL.GetAllByBankIdAsync(bankId);
        }
        public async Task<PagedResult<ServiceModel>> GetPagedByBankIdAsync(
            int bankId, string searchTerm, bool? isActive, int page, int pageSize)
        {
                return await _serviceDAL.GetPagedByBankIdAsync(bankId, searchTerm, isActive, page, pageSize);
        }

        public async Task<List<ServiceModel>> GetByIdsAsync(IEnumerable<int> serviceIds)
        {
                if (serviceIds == null || !serviceIds.Any())
                {
                    return new List<ServiceModel>();
                }

                return await _serviceDAL.GetByIdsAsync(serviceIds);
        }
        public async Task<List<ServiceModel>> GetAllActiveByBankIdAsync(int bankId)
        {
                return await _serviceDAL.GetAllActiveByBankIdAsync(bankId);
        }

        public async Task<ServiceModel?> GetByIdAsync(int Id)
        {
                return await _serviceDAL.GetByIdAsync(Id);
        }

        public async Task<int> CreateAsync(ServiceModel service)
        {
                return await _serviceDAL.CreateAsync(service);
        }

        public async Task<bool> UpdateAsync(ServiceModel service, ServiceModel dbService, bool forceUpdate = false)
        {
            if (Utility.AreObjectsEqual(service, dbService, "RowVersion", "Id", "BankId"))
            {
                return false;
            }

            await _serviceDAL.UpdateAsync(service, forceUpdate);
            return true;

        }

        public async Task DeleteAsync(int id, byte[] rowVersion, bool forceDelete = false)
        {
                await _serviceDAL.DeleteAsync(id, rowVersion, forceDelete);
        }
    }
}
