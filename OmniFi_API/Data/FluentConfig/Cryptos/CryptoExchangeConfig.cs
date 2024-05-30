using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OmniFi_API.Models.Cryptos;
using OmniFi_API.Utilities;

namespace OmniFi_API.Data.FluentConfig.Cryptos
{
    public class CryptoExchangeConfig : IEntityTypeConfiguration<CryptoExchange>
    {
        public void Configure(EntityTypeBuilder<CryptoExchange> builder)
        {
            builder.HasKey(x => x.CryptoExchangeID);

            builder.Property(x => x.ExchangeName)
                .HasMaxLength(50)
                .IsRequired();

            builder.HasData(new CryptoExchange()
            {
                CryptoExchangeID = 1,
                ExchangeName = CryptoExchangeNames.Binance,
                ExchangeLogo = File.ReadAllBytes(".\\Ressources\\Images\\Logos\\BinanceLogo.png")

            });

            builder.HasData(new CryptoExchange()
            {
                CryptoExchangeID = 2,
                ExchangeName = CryptoExchangeNames.CryptoDotCom,
                ExchangeLogo = File.ReadAllBytes(".\\Ressources\\Images\\Logos\\CryptoDotComLogo.png")

            });

            builder.HasData(new CryptoExchange()
            {
                CryptoExchangeID = 3,
                ExchangeName = CryptoExchangeNames.Kraken,
                ExchangeLogo = File.ReadAllBytes(".\\Ressources\\Images\\Logos\\KrakenLogo.png")

            });

        }
    }
}
