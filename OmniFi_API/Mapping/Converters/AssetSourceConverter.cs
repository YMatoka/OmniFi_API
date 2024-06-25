using AutoMapper;
using OmniFi_API.Models.Assets;

namespace OmniFi_API.Mapping.Converters
{
    public class AssetSourceConverter : IValueConverter<AssetSource?, string>
    {
        public string Convert(AssetSource? sourceMember, ResolutionContext context)
        {
            if (sourceMember is null)
                return string.Empty;

            return sourceMember.AssetSourceName;
        }
    }
}
