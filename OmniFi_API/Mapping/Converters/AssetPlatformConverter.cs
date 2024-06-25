using AutoMapper;
using OmniFi_API.Models.Assets;

namespace OmniFi_API.Mapping.Converters
{
    public class AssetPlatformConverter : IValueConverter<AssetPlatform?, string>
    {
        public string Convert(AssetPlatform? sourceMember, ResolutionContext context)
        {
            if (sourceMember is null || sourceMember.CryptoExchange is null && sourceMember.Bank is null)
                return string.Empty;

            if (sourceMember.CryptoExchange is not null)
                return sourceMember.CryptoExchange.ExchangeName;

            if (sourceMember.Bank is not null)
                return sourceMember.Bank.BankName;

            return string.Empty;

        }
    }
}
