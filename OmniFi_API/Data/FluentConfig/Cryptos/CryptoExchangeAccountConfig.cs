using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OmniFi_API.Models.Cryptos;

namespace OmniFi_API.Data.FluentConfig.Cryptos
{
    public class CryptoExchangeAccountConfig : IEntityTypeConfiguration<CryptoExchangeAccount>
    {
        public void Configure(EntityTypeBuilder<CryptoExchangeAccount> builder)
        {
            builder.HasKey(x => x.ExchangeAccountID);

            builder
                .HasOne(x => x.CryptoExchange)
                .WithMany(x => x.cryptoExchangeAccounts)
                .HasForeignKey(x => x.CryptoExchangeID);

            builder
                .HasOne(x => x.User)
                .WithMany(x => x.CryptoExchangeAccounts)
                .HasForeignKey(x => x.UserID);
        }
    }
}
