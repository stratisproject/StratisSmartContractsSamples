using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Signature.DAL.Interface;
using Signature.Shared.Models;
using Signature.Utility.Mapping;

namespace Signature.DAL
{
    public class ContactRepository : IContactRepository
    {
        private readonly IDatabaseSettings _databaseSettings;

        public ContactRepository(IDatabaseSettings databaseSettings)
        {
            _databaseSettings = databaseSettings;
        }

        public async Task<int> AddContact(Contact contact)
        {
            using (var con = new SqlConnection(_databaseSettings.ConnectionString))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "[dbo].[AddContact]";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@FirstName", SqlDbType.VarChar).Value = contact.FirstName;
                    cmd.Parameters.Add("@LastName", SqlDbType.VarChar).Value = contact.LastName;
                    cmd.Parameters.Add("@Email", SqlDbType.VarChar).Value = contact.Email;
                    cmd.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = contact.UserId;
                    cmd.Parameters.Add("@RequestedBy", SqlDbType.UniqueIdentifier).Value = contact.RequestedBy;

                    return await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<IEnumerable<Contact>> GetContacts(Guid userId)
        {
            using (var connection = new SqlConnection(_databaseSettings.ConnectionString))
            {
                await connection.OpenAsync();

                using (var cmd = new SqlCommand("[dbo].[GetContacts]", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = userId;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        var list = new List<Contact>();

                        while (await reader.ReadAsync())
                            list.Add(SqlMapper.Map<Contact>(reader));

                        return list;
                    }
                }
            }
        }

        public async Task<int> DeleteContact(Guid contactId)
        {
            using (var con = new SqlConnection(_databaseSettings.ConnectionString))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "[dbo].[DeleteContact]";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@ContactId", SqlDbType.UniqueIdentifier).Value = contactId;

                    return await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
