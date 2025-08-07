using Bank_Configuration_Portal.BLL.Interfaces;
using Bank_Configuration_Portal.Common;
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
            try
            {
                return await _serviceDAL.GetAllByBankIdAsync(bankId);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "ServiceManager.GetAllByBankIdAsync");
                throw;
            }
        }
        public async Task<List<ServiceModel>> GetAllActiveByBankIdAsync(int bankId)
        {
            try
            {
                return await _serviceDAL.GetAllActiveByBankIdAsync(bankId);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "ServiceManager.GetAllActiveByBankIdAsync");
                throw;
            }
        }

        public async Task<ServiceModel?> GetByIdAsync(int Id)
        {
            try
            {
                return await _serviceDAL.GetByIdAsync(Id);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "ServiceManager.GetByIdAsync");
                throw;
            }
        }

        public async Task<int> CreateAsync(ServiceModel service)
        {
            try
            {
                return await _serviceDAL.CreateAsync(service);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "ServiceManager.AddAsync");
                throw;
            }
        }

        public async Task UpdateAsync(ServiceModel service, bool forceUpdate = false)
        {
            try
            {
                await _serviceDAL.UpdateAsync(service, forceUpdate);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "ServiceManager.UpdateAsync");
                throw;
            }
        }

        public async Task DeleteAsync(int id, byte[] rowVersion, bool forceDelete = false)
        {
            try
            {
                await _serviceDAL.DeleteAsync(id, rowVersion, forceDelete);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "ServiceManager.DeleteAsync");
                throw;
            }
        }
    }
}
