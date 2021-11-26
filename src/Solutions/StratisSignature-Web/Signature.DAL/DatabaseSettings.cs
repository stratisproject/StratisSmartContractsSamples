using Microsoft.Extensions.Configuration;
using Signature.DAL.Interface;

namespace Signature.DAL
{
    public class DatabaseSettings : IDatabaseSettings
    {
        public DatabaseSettings(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public string ConnectionString { get; private set; }
    }
}
