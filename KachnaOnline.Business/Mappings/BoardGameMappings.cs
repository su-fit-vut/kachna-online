// BoardGameMappings.cs
// Author: František Nečas

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
            this.CreateMap<Category, Dto.BoardGames.CategoryDto>();

            // Board games
            this.CreateMap<KachnaOnline.Data.Entities.BoardGames.BoardGame, BoardGame>()
                .ForMember(dest => dest.Category,
                    opt => opt.MapFrom(src => src.Category));
            this.CreateMap<BoardGame, Dto.BoardGames.BoardGameDto>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category));
        }
    }
}
