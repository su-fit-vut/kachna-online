using AutoMapper;
using KachnaOnline.Business.Models.PushNotifications;
using KachnaOnline.Dto.PushNotifications;

namespace KachnaOnline.Business.Mappings
{
    public class PushSubscriptionMappings : Profile
    {
        public PushSubscriptionMappings()
        {
            this.CreateMap<PushSubscriptionDto, PushSubscription>()
                .ForMember(dst => dst.Endpoint, opt => opt.MapFrom(src => src.Subscription.Endpoint))
                .ForMember(dst => dst.BoardGamesEnabled, opt => opt.MapFrom(src => src.Configuration.BoardGamesEnabled))
                .ForMember(dst => dst.StateChangesEnabled,
                    opt => opt.MapFrom(src => src.Configuration.StateChangesEnabled))
                .ForMember(dst => dst.Keys, opt => opt.MapFrom(src => src.Subscription.Keys));

            this.CreateMap<PushSubscription, PushSubscriptionConfiguration>();

            this.CreateMap<PushSubscription, KachnaOnline.Data.Entities.PushSubscriptions.PushSubscription>()
                .ForMember(dst => dst.Keys, opt => opt.Ignore());

            this.CreateMap<KachnaOnline.Data.Entities.PushSubscriptions.PushSubscription, PushSubscription>()
                .ForMember(dst => dst.Keys, opt => opt.Ignore());
        }
    }
}
