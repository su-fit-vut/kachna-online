using System;
using System.Linq;
using AutoMapper;
using KachnaOnline.Business.Configuration;
using KachnaOnline.Business.Models.Kis;
using KachnaOnline.Dto.ClubInfo;
using Microsoft.Extensions.Options;

namespace KachnaOnline.Business.Mappings
{
    public class KisMappings : Profile
    {
        public KisMappings()
        {
            this.CreateMap<KisArticle, OfferedItemDto>()
                .ForMember(dto => dto.Labels, o =>
                    o.MapFrom(a => a.Labels == null ? Array.Empty<string>() : a.Labels.Select(l => l.Name)))
                .ForMember(dto => dto.Prestige, o =>
                    o.MapFrom(a => (int)a.Prestige))
                .ForMember(dto => dto.IsPermanentOffer, o =>
                    o.MapFrom<IsPermanentOfferValueResolver>());

            this.CreateMap<KisLeaderboardItem, LeaderboardItemDto>()
                .ForMember(dto => dto.Prestige, o =>
                    o.MapFrom(li => (int)li.Prestige));
        }
    }

    public class IsPermanentOfferValueResolver : IValueResolver<KisArticle, OfferedItemDto, bool>
    {
        private readonly IOptionsMonitor<KisOptions> _optionsMonitor;

        public IsPermanentOfferValueResolver(IOptionsMonitor<KisOptions> optionsMonitor)
        {
            _optionsMonitor = optionsMonitor;
        }

        public bool Resolve(KisArticle source, OfferedItemDto destination, bool destMember, ResolutionContext context)
        {
            var permanentOfferId = _optionsMonitor.CurrentValue.PermanentOfferLabelId;
            return source.Labels.Any(l => l.Id == permanentOfferId);
        }
    }
}
