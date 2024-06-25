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

            builder
                .Property(x => x.CryptoExchangeID)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.ExchangeName)
                .HasMaxLength(50)
                .IsRequired();

            builder.HasData(new CryptoExchange()
            {
                CryptoExchangeID = 1,
                ExchangeName = CryptoExchangeNames.Binance,
                ImageUrl = "https://w7.pngwing.com/pngs/696/485/png-transparent-binance-logo-cryptocurrency-exchange-coin-text-logo-computer-wallpaper.png"

            });

            builder.HasData(new CryptoExchange()
            {
                CryptoExchangeID = 2,
                ExchangeName = CryptoExchangeNames.CryptoDotCom,
                ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/b/b0/Crypto.com_logo.svg/2560px-Crypto.com_logo.svg.png"

            });

            builder.HasData(new CryptoExchange()
            {
                CryptoExchangeID = 3,
                ExchangeName = CryptoExchangeNames.Kraken,
                ImageUrl = "https://logo-marque.com/wp-content/uploads/2021/03/Kraken-Logo.png"

            });

        }
    }
}
