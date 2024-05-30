using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OmniFi_API.Models.Identity;
using OmniFi_API.Utilities;

namespace OmniFi_API.Data.FluentConfig.Identity
{
    public class ApplicationRoleConfig : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            builder.HasData(new ApplicationRole()
            {
                Name = Roles.Admin
            });

            builder.HasData(new ApplicationRole()
            {
                Name = Roles.User
            });
        }
    }
}
