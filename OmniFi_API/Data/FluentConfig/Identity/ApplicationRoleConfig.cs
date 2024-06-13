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
                Id = "e1f8f0f8-4e98-4d30-abf8-8488705defe3",
                Name = Roles.Admin,
                NormalizedName = Roles.Admin.ToUpper(),
                ConcurrencyStamp = "c833d87b-30cd-4cc0-aa19-9eb5cc2c4b13"
            });

            builder.HasData(new ApplicationRole()
            {
                Id = "ea1104de-92d7-499a-a2eb-0f707e6bb911",
                Name = Roles.User,
                NormalizedName = Roles.User.ToUpper(),
                ConcurrencyStamp = "bbaf7e3d-5d38-4372-bddf-beed77f83baf"
            });
        }
    }
}
