using System;
using System.Linq;
using AutoMapper;
using AutoMapper.Internal;
using KachnaOnline.Business.Models.Cleanings;
using KachnaOnline.Dto.Cleanings;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using CleaningEntity = KachnaOnline.Data.Entities.Cleanings.Cleaning;
using KachnaOnline.Business.Services;

using Microsoft.AspNetCore.SignalR;
using KachnaOnline.Business.Services.Abstractions;
using System.Data.Common;

namespace KachnaOnline.Business.Mappings
{
    public class CleaningMappings : Profile
    {
        private readonly IUserService _userService;

        public CleaningMappings(IUserService userService)
        {
            _userService = userService;

            this.CreateMap<CleaningEntity, Cleaning>();

            this.CreateMap<NewCleaning, CleaningEntity>();
            this.CreateMap<ModifiedCleaning, CleaningEntity>();

            this.CreateMap<Cleaning, CleaningDto>();
            this.CreateMap<Cleaning, ManagerCleaningDto>();

            this.CreateMap<BaseCleaningDto, NewCleaning>();
            this.CreateMap<BaseCleaningDto, ModifiedCleaning>();
        }
    }
}
