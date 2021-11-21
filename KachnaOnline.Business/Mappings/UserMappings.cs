// UserMappings.cs
// Author: Ondřej Ondryáš

using AutoMapper;
using KachnaOnline.Business.Models.Users;
using KachnaOnline.Dto.Roles;

namespace KachnaOnline.Business.Mappings
{
    public class UserMappings : Profile
    {
        public UserMappings()
        {
            this.CreateMap<KachnaOnline.Data.Entities.Users.User, User>();
            
            // Roles
            this.CreateMap<KachnaOnline.Data.Entities.Users.Role, Role>().ReverseMap();
            this.CreateMap<Role, RoleDto>();
        }
    }
}
