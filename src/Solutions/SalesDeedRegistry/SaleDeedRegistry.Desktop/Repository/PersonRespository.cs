using SQLite.Net;
using System.Linq;
using SQLite.Net.Platform.Win32;
using SaleDeedRegistry.Lib.Entities;
using System.Collections.Generic;

namespace SaleDeedRegistry.Desktop.Repository
{
    /// <summary>
    /// The Person Repository Responsible for managing the Asset or Property
    /// Person Information
    /// </summary>
    public class PersonRespository : SQLiteConnection
    {
        private readonly string path;
        private readonly TableRespository tableRespository;

        public PersonRespository(string path) : base(new SQLitePlatformWin32(), path)
        {
            this.path = path;
            tableRespository = new TableRespository(path);
            if (!tableRespository.TableExists<PersonInfo>())
            {
                CreateTable<PersonInfo>();
            }
        }

        /// Query PersonInfo by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>PersonInfo</returns>
        public PersonInfo Query(int id)
        {
            return (from personInfo in Table<PersonInfo>()
                    where personInfo.Id == id
                    select personInfo).FirstOrDefault();
        }

        /// <summary>
        ///  Query all the Persons
        /// </summary>
        /// <returns>Collection of PersonInfo</returns>
        public List<PersonInfo> QueryAllPersons()
        {
            return (from person in Table<PersonInfo>()
                    orderby person.Id
                    select person).ToList();
        }

        /// <summary>
        /// Search or Filter Person
        /// </summary>
        /// <param name="personInfo">PersonInfo</param>
        /// <returns>Collection of PersonInfo</returns>
        public List<PersonInfo> Search(PersonInfo personInfo)
        {
            var db = new SQLiteConnection(new SQLitePlatformWin32(), path);
            var personTable = Table<PersonInfo>();

            if(!string.IsNullOrEmpty(personInfo.FirstName))
                personTable = personTable.Where(p => p.FirstName.ToLower() == personInfo.FirstName.ToLower());

            if (!string.IsNullOrEmpty(personInfo.LastName))
                personTable = personTable.Where(p => p.LastName.ToLower() == personInfo.LastName.ToLower());

            if (!string.IsNullOrEmpty(personInfo.Aaddhar))
                personTable = personTable.Where(p => p.Aaddhar.ToLower() == personInfo.Aaddhar.ToLower());

            if (!string.IsNullOrEmpty(personInfo.PAN))
                personTable = personTable.Where(p => p.PAN.ToLower() == personInfo.PAN.ToLower());

            return personTable.ToList();
        }

        public int GetLastId()
        {
            var db = new SQLiteConnection(new SQLitePlatformWin32(), path);
            return db.ExecuteScalar<int>("SELECT rowid from PersonInfo order" +
                " by ROWID DESC limit 1");
        }
    }
}
