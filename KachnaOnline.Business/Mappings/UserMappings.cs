// UserMappings.cs
// Author: Ondřej Ondryáš

using AutoMapper;
using KachnaOnline.Business.Models.Users;

namespace KachnaOnline.Business.Mappings
{
    public class UserMappings : Profile
    {
        public UserMappings()
        {
            this.CreateMap<KachnaOnline.Data.Entities.Users.User, User>();
        }
    }
}
