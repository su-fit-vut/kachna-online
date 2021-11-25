// UserMappings.cs
// Author: Ondřej Ondryáš

using AutoMapper;
using KachnaOnline.Business.Models.Users;
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
            this.CreateMap<User, UserDetailsDto>()
                .IncludeBase<User, UserDto>()
                .ForMember(dto => dto.ManuallyAssignedRoles, options =>
                    options.Ignore())
                .ForMember(dto => dto.ActiveRoles, options =>
                    options.Ignore());
            this.CreateMap<RoleAssignment, UserRoleAssignmentDetailsDto>();
        }
    }
}
