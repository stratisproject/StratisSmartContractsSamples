using System.Threading.Tasks;
using Signature.Shared.Models;

namespace Signature.DAL.Interface
{
    public interface IUserRepository
    {
        Task<int> AddUser(User userModel);

        Task<User> GetUserByEmail(string email);
    }
}
