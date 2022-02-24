using System.Linq;
using AutoMapper;
using AutoMapper.Internal;
using KachnaOnline.Business.Models.Events;
using KachnaOnline.Dto.Events;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using EventEntity = KachnaOnline.Data.Entities.Events.Event;

namespace KachnaOnline.Business.Mappings
{
    public class EventMappings : Profile
    {
        public EventMappings()
        {
            this.CreateMap<EventEntity, Event>()
                .ForMember(e => e.LinkedPlannedStateIds, options =>
                    options.MapFrom(e => e.LinkedPlannedStates.Select(state => state.Id).ToList()));

            this.CreateMap<EventEntity, EventWithLinkedStates>()
                .ForMember(e => e.LinkedStates, options =>
                    options.MapFrom(e => e.LinkedPlannedStates))
                .ForMember(e => e.LinkedPlannedStateIds, options =>
                    options.MapFrom(e => e.LinkedPlannedStates.Select(state => state.Id).ToList()));

            this.CreateMap<NewEvent, EventEntity>();
            this.CreateMap<ModifiedEvent, EventEntity>();

            this.CreateMap<Event, EventDto>();
            this.CreateMap<Event, ManagerEventDto>();

            this.CreateMap<EventWithLinkedStates, EventWithLinkedStatesDto>()
                .ForMember(e => e.LinkedStatesDtos, options =>
                    options.MapFrom(e => e.LinkedStates));
            this.CreateMap<EventWithLinkedStates, ManagerEventWithLinkedStatesDto>()
                .ForMember(e => e.LinkedStatesDtos, options =>
                    options.MapFrom(e => e.LinkedStates));

            this.CreateMap<BaseEventDto, NewEvent>();
            this.CreateMap<BaseEventDto, ModifiedEvent>();
        }
    }
}
