using SQLite.Net;
using SQLite.Net.Platform.Win32;

namespace SaleDeedRegistry.Desktop.Repository
{
    public class TableRespository : SQLiteConnection
    {
        private readonly string dbPath = "";

        public TableRespository(string path) : 
            base(new SQLitePlatformWin32(), path)
        {
            this.dbPath = path;
        }

        /// <summary>
        /// Check if the table exist or not
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>True/False</returns>
        public bool TableExists<T>()
        {
            var db = new SQLiteConnection(new SQLitePlatformWin32(), dbPath);
            const string cmdText = "SELECT name FROM sqlite_master WHERE type='table' AND name=?";
            var cmd = db.CreateCommand(cmdText, typeof(T).Name);
            return cmd.ExecuteScalar<string>() != null;
        }
    }
}
