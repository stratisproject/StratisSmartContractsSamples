using System.Linq;
using SQLite.Net;
using SQLite.Net.Platform.Win32;
using SaleDeedRegistry.Lib.Entities;
using System.Collections.Generic;

namespace SaleDeedRegistry.Desktop.Repository
{ 
    /// <summary>
    /// The Asset Management Repository deals with the Contact 
    ///     Add
    ///     Update
    ///     Delete
    ///     Searches
    /// </summary>
    public class AssetManagementRepository : SQLiteConnection
    {
        private readonly string dbPath;
        private readonly TableRespository tableRespository;

        public AssetManagementRepository(string path) : base(new SQLitePlatformWin32(), path)
        {
            this.dbPath = path;
            tableRespository = new TableRespository(path);
            if (!tableRespository.TableExists<AssetInfo>())
            {
                CreateTable<AssetInfo>();
            }
        }

        /// <summary>
        /// Insert AssetInfo
        /// </summary>
        /// <param name="assetInfo"></param>
        /// <returns></returns>
        public int InsertAsset(AssetInfo assetInfo)
        {
            return this.Insert(assetInfo);
        }

        /// <summary>
        /// Update AssetInfo
        /// </summary>
        /// <param name="assetInfo"></param>
        /// <returns></returns>
        public int UpdateAsset(AssetInfo assetInfo)
        {
            return this.Update(assetInfo);
        }

        /// <summary>
        /// Delete an AssetInfo by Id
        /// </summary>
        /// <param name="id"></param>
        public void DeleteAsset(int id)
        {
            var asset = QueryAssetById(id);
            Delete(asset);
        }

        /// <summary>
        /// Delete or Cleanup All the Assets
        /// </summary>
        public void DeleteAllAssets()
        {
            this.DeleteAll<AssetInfo>();
        }

        /// <summary>
        /// Search AssetInfo by AssetId and Property Number
        /// </summary>
        /// <param name="assetId">AssetId</param>
        /// <param name="propertyNumber">PropertyNumber</param>
        /// <returns>Collection of AssetInfo</returns>
        public List<AssetInfo> Search(string assetId, string propertyNumber)
        {
            var db = new SQLiteConnection(new SQLitePlatformWin32(), dbPath);
            var assetTable = Table<AssetInfo>();

            if (!string.IsNullOrEmpty(assetId))
                assetTable = assetTable.Where(p => p.AssetId == assetId);

            if (!string.IsNullOrEmpty(propertyNumber))
                assetTable = assetTable.Where(p => p.PropertyNumber.ToLower() == propertyNumber);

            return assetTable.ToList();
        }

        /// <summary>
        /// Query Assets by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>AssetInfo</returns>
        public AssetInfo QueryAssetById(int id)
        {
            return (from asset in Table<AssetInfo>()
                    where asset.Id == id
                    select asset).FirstOrDefault();
        }

        /// <summary>
        /// Query Assets by PersonId
        /// </summary>
        /// <param name="personId">personId</param>
        /// <returns>AssetInfo</returns>
        public List<AssetInfo> QueryAssetByPersonId(int personId)
        {
            return (from asset in Table<AssetInfo>()
                    where asset.PersonId == personId
                    select asset).ToList();
        }

        /// <summary>
        /// Query Assets by the AssetId
        /// </summary>
        /// <param name="assetId">AssetId</param>
        /// <returns>AssetInfo</returns>
        public AssetInfo QueryAssetsByAssetId(string assetId)
        {
            return (from asset in Table<AssetInfo>()
                    orderby asset.Id
                    where asset.AssetId == assetId
                    select asset).FirstOrDefault();
        }

        /// <summary>
        ///  Query all the Assets
        /// </summary>
        /// <returns>Collection of AssetInfo</returns>
        public List<AssetInfo> QueryAllAssets()
        {
            return (from asset in Table<AssetInfo>()
                   orderby asset.Id
                   select asset).ToList();
        }
    }
}
