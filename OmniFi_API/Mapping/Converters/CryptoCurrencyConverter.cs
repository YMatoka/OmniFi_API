using AutoMapper;
using OmniFi_API.Models.Cryptos;

namespace OmniFi_API.Mapping.Converters
{
    public class CryptoCurrencyConverter : IValueConverter<CryptoCurrency?, string>
    {
        public string Convert(CryptoCurrency sourceMember, ResolutionContext context)
        {
            return sourceMember is not null ? 
                sourceMember.CurrencySymbol :
                string.Empty;
        }
    }
}
