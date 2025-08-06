using Bank_Configuration_Portal.BLL.Interfaces;
using Bank_Configuration_Portal.Common;
using Bank_Configuration_Portal.DAL.Interfaces;
using Bank_Configuration_Portal.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Configuration_Portal.BLL
{
    public class CounterManager : ICounterManager
    {
        private readonly ICounterDAL _counterDAL;

        public CounterManager(ICounterDAL counterDAL)
        {
            _counterDAL = counterDAL;
        }

        public async Task<List<CounterModel>> GetAllByBranchIdAsync(int branchId)
        {
            try
            {
                return await _counterDAL.GetAllByBranchIdAsync(branchId);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "CounterManager.GetAllByBranchIdAsync");
                throw;
            }
        }

        public async Task<CounterModel?> GetByIdAsync(int id)
        {
            try
            {
                var counter = await _counterDAL.GetByIdAsync(id);

                if (counter != null)
                {
                    counter.AllocatedServiceIds = await _counterDAL.GetAllocatedServiceIdsAsync(id);
                }
                return counter;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "CounterManager.GetByIdAsync");
                throw;
            }
        }

        public async Task<int> CreateAsync(CounterModel counter)
        {
            try
            {
                int newCounterId = await _counterDAL.CreateAsync(counter);

                if (newCounterId > 0 && counter.AllocatedServiceIds != null)
                {
                    await _counterDAL.SaveAllocationsAsync(newCounterId, counter.AllocatedServiceIds);
                }
                return newCounterId;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "CounterManager.UpdateAsync");
                throw;
            }

        }

        public async Task UpdateAsync(CounterModel counter, bool forceUpdate = false)
        {
            try
            {
                await _counterDAL.UpdateAsync(counter, forceUpdate);

                if (counter.AllocatedServiceIds != null)
                {
                    await _counterDAL.SaveAllocationsAsync(counter.Id, counter.AllocatedServiceIds);
                }
                else
                {
                    await _counterDAL.DeleteAllocationsByCounterIdAsync(counter.Id);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "CounterManager.UpdateAsync");
                throw;
            }
        }

        public async Task DeleteAsync(int id, byte[] rowVersion)
        {
            try
            {
                await _counterDAL.DeleteAsync(id, rowVersion);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "CounterManager.DeleteAsync");
                throw;
            }
        }
    }

}
