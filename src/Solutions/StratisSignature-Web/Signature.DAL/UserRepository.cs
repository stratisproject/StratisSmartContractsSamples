using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Signature.DAL.Interface;
using Signature.Shared.Models;
using Signature.Utility.Mapping;

namespace Signature.DAL
{
    public class UserRepository : IUserRepository
    {
        private readonly IDatabaseSettings _databaseSettings;

        public UserRepository(IDatabaseSettings databaseSettings)
        {
            _databaseSettings = databaseSettings;
        }

        public async Task<int> AddUser(User userModel)
        {

            using (var con = new SqlConnection(_databaseSettings.ConnectionString))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "[dbo].[RegisterUser]";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@FirstName", SqlDbType.VarChar).Value = userModel.FirstName;
                    cmd.Parameters.Add("@LastName", SqlDbType.VarChar).Value = userModel.LastName;
                    cmd.Parameters.Add("@Email", SqlDbType.VarChar).Value = userModel.Email;
                    cmd.Parameters.Add("@Password", SqlDbType.VarChar).Value = userModel.Password;
                    cmd.Parameters.Add("@WalletAddress", SqlDbType.VarChar).Value = userModel.WalletAddress;

                    return await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<User> GetUserByEmail(string email)
        {
            using (var con = new SqlConnection(_databaseSettings.ConnectionString))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "[dbo].[GetUserByEmail]";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@Email", SqlDbType.VarChar).Value = email;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        var user = new User();

                        while (await reader.ReadAsync())
                            user = SqlMapper.Map<User>(reader);

                        return user;
                    }
                }
            }
        }
    }
}
