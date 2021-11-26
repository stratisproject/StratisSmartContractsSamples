using SQLite.Net;
using SQLite.Net.Platform.Win32;
using SaleDeedRegistry.Lib.Entities;

namespace SaleDeedRegistry.Desktop.Repository
{
    /// <summary>
    /// The Location Repository responsible for managing the 
    /// Person or Asset/Property Location Info
    /// </summary>
    public class LocationRespository : SQLiteConnection
    {
        private readonly string path;
        private readonly TableRespository tableRespository;

        public LocationRespository(string path) : base(new SQLitePlatformWin32(), path)
        {
            this.path = path;
            tableRespository = new TableRespository(path);
            if (!tableRespository.TableExists<LocationInfo>())
            {
                CreateTable<LocationInfo>();
            }
        }

        /// Query LocationInfo by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>LocationInfo</returns>
        public LocationInfo Query(int id)
        {
            return (from locationInfo in Table<LocationInfo>()
                    where locationInfo.Id == id
                    select locationInfo).FirstOrDefault();
        }

        public int GetLastId()
        {
            var db = new SQLiteConnection(new SQLitePlatformWin32(), path);
            return db.ExecuteScalar<int>("SELECT rowid from LocationInfo order" +
                " by ROWID DESC limit 1");
        }
    }
}
