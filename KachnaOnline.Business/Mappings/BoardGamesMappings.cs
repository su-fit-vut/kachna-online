// BoardGamesMappings.cs
// Author: František Nečas

using System;
using AutoMapper;
using KachnaOnline.Business.Models.BoardGames;
using KachnaOnline.Dto.BoardGames;
using ReservationEventType = KachnaOnline.Business.Models.BoardGames.ReservationEventType;
using ReservationItemState = KachnaOnline.Business.Models.BoardGames.ReservationItemState;
using ReservationState = KachnaOnline.Business.Models.BoardGames.ReservationState;

namespace KachnaOnline.Business.Mappings
{
    public class BoardGamesMappings : Profile
    {
        public BoardGamesMappings()
        {
            // Categories
            this.CreateMap<KachnaOnline.Data.Entities.BoardGames.Category, Category>().ReverseMap();
            this.CreateMap<Category, CategoryDto>().ReverseMap();

            // Board games
            this.CreateMap<KachnaOnline.Data.Entities.BoardGames.BoardGame, BoardGame>();
            this.CreateMap<BoardGame, KachnaOnline.Data.Entities.BoardGames.BoardGame>()
                .ForMember(dst => dst.Id, opt => opt.Ignore());
            this.CreateMap<BoardGame, BoardGameDto>();

            // Board games - manager and creation
            this.CreateMap<BoardGame, ManagerBoardGameDto>()
                .ForMember(
                    dst => dst.DefaultReservationDays,
                    opt => opt.MapFrom<int?>(src =>
                        src.DefaultReservationTime == null
                            ? null
                            : src.DefaultReservationTime.Value.Days));
            this.CreateMap<CreateBoardGameDto, BoardGame>()
                .ForMember(
                    dst => dst.DefaultReservationTime,
                    opt => opt.MapFrom<TimeSpan?>(src =>
                        src.DefaultReservationDays == null
                            ? null
                            : TimeSpan.FromDays(src.DefaultReservationDays.Value)));

            // Reservations
            // Entities <-> Models
            this.CreateMap<KachnaOnline.Data.Entities.BoardGames.Reservation, Reservation>().ReverseMap();
            this.CreateMap<KachnaOnline.Data.Entities.BoardGames.ReservationItem, ReservationItem>().ReverseMap();

            this.CreateMap<KachnaOnline.Data.Entities.BoardGames.ReservationItemEvent, ReservationItemEvent>()
                .ReverseMap();
            this.CreateMap<KachnaOnline.Data.Entities.BoardGames.ReservationEventType, ReservationEventType>()
                .ReverseMap();
            this.CreateMap<KachnaOnline.Data.Entities.BoardGames.ReservationItemState, ReservationItemState>()
                .ReverseMap();

            // DTOs
            this.CreateMap<Reservation, ReservationDto>();
            this.CreateMap<Reservation, ManagerReservationDto>();
            this.CreateMap<KachnaOnline.Dto.BoardGames.ReservationState, ReservationState>().ReverseMap();

            this.CreateMap<ReservationItem, ReservationItemDto>();
            this.CreateMap<ReservationItem, ManagerReservationDto>();
            this.CreateMap<KachnaOnline.Dto.BoardGames.ReservationItemState, ReservationItemState>().ReverseMap();

            this.CreateMap<ReservationItemEvent, ReservationItemEventDto>();
            this.CreateMap<KachnaOnline.Dto.BoardGames.ReservationEventType, ReservationEventType>().ReverseMap();

            this.CreateMap<CreateReservationDto, Reservation>()
                .ForMember(dst => dst.MadeOn, opt => opt.MapFrom(src => DateTime.Now));
            this.CreateMap<ManagerCreateReservationDto, Reservation>()
                .ForMember(dst => dst.MadeOn, opt => opt.MapFrom(src => DateTime.Now));
        }
    }
}
