using Bank_Configuration_Portal.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Configuration_Portal.BLL.Interfaces
{
    public interface IServiceManager
    {
        Task<List<ServiceModel>> GetAllByBankIdAsync(int bankId);
        Task<ServiceModel?> GetByIdAsync(int id);
        Task<int> CreateAsync(ServiceModel service);
        Task UpdateAsync(ServiceModel service, bool forceUpdate = false);
        Task DeleteAsync(int id, byte[] rowversion);
    }
}
