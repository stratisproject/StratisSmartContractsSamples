using System.Threading.Tasks;
using Signature.Shared.Models;

namespace Signature.BAL.Interface
{
    public interface IUserService
    {
        Task<int> RegisterUser(User registerUser);

        Task<User> GetUserByEmail(string email);
    }
}
