using AutoMapper;
using OmniFi_API.Models.Currencies;

namespace OmniFi_API.Mapping.Converters
{
    public class FiatCurrencyConverter : IValueConverter<FiatCurrency?, string>
    {
        public string Convert(FiatCurrency? sourceMember, ResolutionContext context)
        {
            if (sourceMember is null)
                return string.Empty;

            return sourceMember.CurrencyCode;
        }
    }
}
