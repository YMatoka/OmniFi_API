namespace OmniFi_API.Models.Assets
{
    public class AssetSource
    {
        public int AssetSourceID { get; set; }
        public required string AssetSourceName { get; set;}
        public ICollection<FinancialAsset>? FinancialAssets { get; set; }
        public ICollection<FinancialAssetHistory>? FinancialAssetsHistory { get; set; }
    }
}
