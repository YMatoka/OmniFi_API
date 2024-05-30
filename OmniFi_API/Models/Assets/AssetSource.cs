namespace OmniFi_API.Models.Assets
{
    public class AssetSource
    {
        public int AssetSourceID { get; set; }
        public required string AssetSourceName { get; set;}
        public ICollection<AssetTracking>? AssetTrackings { get; set; }
    }
}
