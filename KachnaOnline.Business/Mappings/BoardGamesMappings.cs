// BoardGamesMappings.cs
// Author: František Nečas

using System;
using AutoMapper;
using KachnaOnline.Business.Models.BoardGames;

namespace KachnaOnline.Business.Mappings
{
    public class BoardGamesMappings : Profile
    {
        public BoardGamesMappings()
        {
            // Categories
            this.CreateMap<KachnaOnline.Data.Entities.BoardGames.Category, Category>();
            this.CreateMap<Category, KachnaOnline.Data.Entities.BoardGames.Category>();
            this.CreateMap<Category, Dto.BoardGames.CategoryDto>();
            this.CreateMap<Dto.BoardGames.CreateCategoryDto, Category>();

            // Board games
            this.CreateMap<KachnaOnline.Data.Entities.BoardGames.BoardGame, BoardGame>();
            this.CreateMap<BoardGame, KachnaOnline.Data.Entities.BoardGames.BoardGame>()
                .ForMember(dst => dst.Id, opt => opt.Ignore());
            this.CreateMap<BoardGame, Dto.BoardGames.BoardGameDto>();
            
            // Board games - manager and creation
            this.CreateMap<BoardGame, Dto.BoardGames.ManagerBoardGameDto>()
                .ForMember(
                    dst => dst.DefaultReservationDays,
                    opt => opt.MapFrom<int?>(src =>
                        src.DefaultReservationTime == null
                            ? null
                            : src.DefaultReservationTime.Value.Days));
            this.CreateMap<Dto.BoardGames.CreateBoardGameDto, BoardGame>()
                .ForMember(
                    dst => dst.DefaultReservationTime,
                    opt => opt.MapFrom<TimeSpan?>(src =>
                        src.DefaultReservationDays == null
                            ? null
                            : TimeSpan.FromDays(src.DefaultReservationDays.Value)));
        }
    }
}
