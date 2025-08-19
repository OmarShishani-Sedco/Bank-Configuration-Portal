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
                return await _counterDAL.GetAllByBranchIdAsync(branchId);
        }

        public async Task<CounterModel?> GetByIdAsync(int id)
        {
                var counter = await _counterDAL.GetByIdAsync(id);

                if (counter != null)
                {
                    counter.AllocatedServiceIds = await _counterDAL.GetAllocatedServiceIdsAsync(id);
                }
                return counter;
        }

        public async Task<int> CreateAsync(CounterModel counter)
        {
                int newCounterId = await _counterDAL.CreateAsync(counter);

                if (newCounterId > 0 && counter.AllocatedServiceIds != null)
                {
                    await _counterDAL.SaveAllocationsAsync(newCounterId, counter.AllocatedServiceIds);
                }
                return newCounterId;
        }

        public async Task<bool> UpdateAsync(CounterModel counter, CounterModel dbCounter, bool forceUpdate = false)
        {
            if (Utility.AreObjectsEqual(counter, dbCounter, "RowVersion", "Id", "BankId"))
            {
                return false;
            }

            await _counterDAL.UpdateAsync(counter, forceUpdate);

                if (counter.AllocatedServiceIds != null)
                {
                    await _counterDAL.SaveAllocationsAsync(counter.Id, counter.AllocatedServiceIds);
                }
                else
                {
                    await _counterDAL.DeleteAllocationsByCounterIdAsync(counter.Id);
                }
                return true;
        }

        public async Task DeleteAsync(int id, byte[] rowVersion, bool forceDelete = false)
        {
                await _counterDAL.DeleteAsync(id, rowVersion, forceDelete);
        }
    }

}
