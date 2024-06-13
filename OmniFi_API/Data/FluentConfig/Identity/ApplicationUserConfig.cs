using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OmniFi_API.Models;
using OmniFi_API.Models.Identity;

namespace OmniFi_API.Data.FluentConfig.Identity
{
    public class ApplicationUserConfig : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {

            builder
                .Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder
                .Property(x => x.FirstName)
                .HasMaxLength (50)
                .IsRequired();

            builder
                .Property(x => x.LastName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.CreatedAt).IsRequired();

            builder
                .HasOne(x => x.FiatCurrency)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.SelectedFiatCurrencyID);

        }
    }
}
