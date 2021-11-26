using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace SaleDeedRegistry.Lib.Entities
{
    [Table("PersonInfo")] 
    public class PersonInfo
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }

        [OneToOne]
        [ForeignKey(typeof(LocationInfo))]
        public int LocationId { get; set; }

        public string Signature { get; set; }
        public string WalletAddress { get; set; } // The Wallet Address
        public string PAN { get; set; } // Unique identification number for Tax purpose
        public string Aaddhar { get; set; } // Unique identification number like SSN
    }
}
