using Bank_Configuration_Portal.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Configuration_Portal.DAL.Interfaces
{
    public interface ICounterDAL
    {
        Task<List<CounterModel>> GetAllByBranchIdAsync(int branchId);
        Task<CounterModel?> GetByIdAsync(int id);
        Task<int> CreateAsync(CounterModel counter);
        Task UpdateAsync(CounterModel counter, bool forceUpdate = false);
        Task DeleteAsync(int id, byte[] rowVersion);
        Task<List<int>> GetAllocatedServiceIdsAsync(int counterId);
        Task SaveAllocationsAsync(int counterId, List<int> serviceIds);
        Task DeleteAllocationsByCounterIdAsync(int counterId);
    }

}
