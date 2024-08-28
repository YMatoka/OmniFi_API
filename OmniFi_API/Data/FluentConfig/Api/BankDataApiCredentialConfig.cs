using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OmniFi_API.Models.Api;

namespace OmniFi_API.Data.FluentConfig.Api
{
    public class BankDataApiCredentialConfig : IEntityTypeConfiguration<BankDataApiCredential>
    {
        public void Configure(EntityTypeBuilder<BankDataApiCredential> builder)
        {
            builder.HasKey(x => x.Id);

            builder
                .Property( x=> x.Id)
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder
                .Property( x=> x.UserId)
                .IsRequired();

           

            
        }
    }
}
