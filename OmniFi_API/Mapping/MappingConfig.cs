using AutoMapper;
using OmniFi_API.Dtos.Identity;
using OmniFi_API.Models.Identity;

namespace OmniFi_API.Mapping
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<ApplicationUser, UserDTO>().ReverseMap();
        }
    }
}
