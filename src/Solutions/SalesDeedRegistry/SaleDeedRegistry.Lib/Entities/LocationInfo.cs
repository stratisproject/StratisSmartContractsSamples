using SQLite.Net.Attributes;

namespace SaleDeedRegistry.Lib.Entities
{
    [Table("LocationInfo")] // SQLite attribute
    public class LocationInfo
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}
