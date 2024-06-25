using AutoMapper;
using OmniFi_API.Models.Identity;

namespace OmniFi_API.Mapping.Converters
{
    public class UserConverter : IValueConverter<ApplicationUser?, string>
    {
        public string Convert(ApplicationUser? sourceMember, ResolutionContext context)
        {
            if (sourceMember is null || sourceMember.UserName is null)
                return string.Empty;

            return sourceMember.UserName;

        }
    }
}
