using Bank_Configuration_Portal.Models.Models;
using System.Threading.Tasks;

namespace Bank_Configuration_Portal.BLL.Interfaces
{
    public interface IBankManager
    {
        Task<BankModel?> GetByNameAsync(string name);
        Task<bool> IsUserMappedToBankAsync(string username, int bankId);
    }
}
