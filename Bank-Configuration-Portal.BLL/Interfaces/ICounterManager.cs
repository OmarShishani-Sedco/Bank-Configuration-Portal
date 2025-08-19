using Bank_Configuration_Portal.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Configuration_Portal.BLL.Interfaces
{
    public interface ICounterManager
    {
        Task<List<CounterModel>> GetAllByBranchIdAsync(int branchId);
        Task<CounterModel?> GetByIdAsync(int id);
        Task<int> CreateAsync(CounterModel counter);
        Task<bool> UpdateAsync(CounterModel counter, CounterModel dbCounter, bool forceUpdate = false);
        Task DeleteAsync(int id, byte[] rowVersion, bool forceDelete = false);
    }

}
