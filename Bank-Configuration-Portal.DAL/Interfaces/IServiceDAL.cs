using Bank_Configuration_Portal.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Configuration_Portal.DAL.Interfaces
{
    public interface IServiceDAL
    {
        Task<List<ServiceModel>> GetAllByBankIdAsync(int bankId);
        Task<List<ServiceModel>> GetAllActiveByBankIdAsync(int bankId);
        Task<ServiceModel?> GetByIdAsync(int serviceId);
        Task<int> CreateAsync(ServiceModel service);
        Task UpdateAsync(ServiceModel service, bool forceUpdate = false);
        Task DeleteAsync(int serviceId, byte[] rowVersion, bool forceDelete = false);
    }
}
