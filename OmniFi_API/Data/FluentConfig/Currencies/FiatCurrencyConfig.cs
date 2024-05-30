using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OmniFi_API.Models.Currencies;
using OmniFi_API.Utilities;

namespace OmniFi_API.Data.FluentConfig.Currencies
{
    public class FiatCurrencyConfig : IEntityTypeConfiguration<FiatCurrency>
    {
        public void Configure(EntityTypeBuilder<FiatCurrency> builder)
        {
            builder.HasKey(x => x.CurrencyID);


            builder
                .Property(x => x.CurrencyCode)
                .HasMaxLength(3)
                .IsRequired();

            builder
                .Property(x => x.CurrencyName)
                .HasMaxLength(50)
                .IsRequired();

            builder
                .Property(x => x.CurrencySymbol)
                .HasMaxLength(3)
                .IsRequired();

            var data = new List<FiatCurrency>(){
                new FiatCurrency
                {
                    CurrencyID = 1,
                    CurrencyCode = FiatCurrencyCodes.USD,
                    CurrencyName = "United States Dollar",
                    CurrencySymbol = "$"
                },
                new FiatCurrency
                {
                    CurrencyID = 2,
                    CurrencyCode = FiatCurrencyCodes.EUR,
                    CurrencyName = "Euro",
                    CurrencySymbol = "€"
                },
                new FiatCurrency
                {

                    CurrencyID = 3,
                    CurrencyCode = FiatCurrencyCodes.GBP,
                    CurrencyName = "British Pound",
                    CurrencySymbol = "£"
                },
                new FiatCurrency
                {

                    CurrencyID = 4,
                    CurrencyCode = FiatCurrencyCodes.CHF,
                    CurrencyName = "Swiss Franc",
                    CurrencySymbol = "₣"
                },
                new FiatCurrency
                {
                    CurrencyID = 5,
                    CurrencyCode = FiatCurrencyCodes.JPY,
                    CurrencyName = "Japanese Yen",
                    CurrencySymbol = "¥"
                }
            };

            builder.HasData(data);

        }
    }
}
