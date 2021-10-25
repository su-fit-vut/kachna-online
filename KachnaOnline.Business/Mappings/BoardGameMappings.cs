// BoardGameMappings.cs
// Author: František Nečas

using System;
using AutoMapper;
using KachnaOnline.Business.Models.BoardGames;

namespace KachnaOnline.Business.Mappings
{
    public class BoardGameMappings : Profile
    {
        public BoardGameMappings()
        {
            // Categories
            this.CreateMap<KachnaOnline.Data.Entities.BoardGames.Category, Category>();
            this.CreateMap<Category, KachnaOnline.Data.Entities.BoardGames.Category>();
            this.CreateMap<Category, Dto.BoardGames.CategoryDto>();
            this.CreateMap<Dto.BoardGames.CategoryDto, Category>()
                .ForMember(x => x.Id, opt => opt.Ignore());

            // Board games
            this.CreateMap<KachnaOnline.Data.Entities.BoardGames.BoardGame, BoardGame>();
            this.CreateMap<BoardGame, KachnaOnline.Data.Entities.BoardGames.BoardGame>();
            this.CreateMap<BoardGame, Dto.BoardGames.BoardGameDto>()
                .ForMember(x => x.CategoryId, opt => opt.Ignore())
                .ForMember(
                    dst => dst.DefaultReservationDays,
                    opt => opt.MapFrom<int?>(src =>
                        src.DefaultReservationTime == null
                            ? null
                            : src.DefaultReservationTime.Value.Days));
            this.CreateMap<Dto.BoardGames.BoardGameDto, BoardGame>()
                .ForMember(
                    dst => dst.DefaultReservationTime,
                    opt => opt.MapFrom<TimeSpan?>(src =>
                        src.DefaultReservationDays == null
                            ? null
                            : new TimeSpan(src.DefaultReservationDays.Value, 0, 0, 0)));
        }
    }
}
