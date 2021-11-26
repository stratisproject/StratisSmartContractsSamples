using System.Threading.Tasks;
using Signature.BAL.Interface;
using Signature.DAL.Interface;
using Signature.Shared.Models;

namespace Signature.BAL
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<int> RegisterUser(User registerUser)
        {
            return await _userRepository.AddUser(registerUser);
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _userRepository.GetUserByEmail(email);
        }
    }
}
