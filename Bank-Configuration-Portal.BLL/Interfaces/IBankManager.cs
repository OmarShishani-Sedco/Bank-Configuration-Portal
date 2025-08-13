using Bank_Configuration_Portal.Models.Models;
using System.Threading.Tasks;

namespace Bank_Configuration_Portal.BLL.Interfaces
{
    public interface IBankManager
    {
        Task<bool> BankExistsAsync(int bankId);
        Task<BankModel?> GetByNameAsync(string name);
        Task<bool> IsUserMappedToBankAsync(string username, int bankId);
    }
}
