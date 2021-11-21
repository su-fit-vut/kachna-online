// UserMappings.cs
// Author: Ondřej Ondryáš

using AutoMapper;
using KachnaOnline.Business.Models.Users;
using KachnaOnline.Dto.Roles;
using KachnaOnline.Dto.Users;

namespace KachnaOnline.Business.Mappings
{
    public class UserMappings : Profile
    {
        public UserMappings()
        {
            // Users
            this.CreateMap<KachnaOnline.Data.Entities.Users.User, User>();
            this.CreateMap<User, UserDto>();
            
            // Roles
            this.CreateMap<KachnaOnline.Data.Entities.Users.Role, Role>().ReverseMap();
            this.CreateMap<Role, RoleDto>();
            this.CreateMap<UserRole, KachnaOnline.Data.Entities.Users.UserRole>().ReverseMap();
        }
    }
}
