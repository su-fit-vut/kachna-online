// EventsMappings.cs
// Author: David Chocholatý

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
            this.CreateMap<Event, EventEntity>();

            this.CreateMap<EventEntity, EventWithLinkedStates>()
                .ForMember(e => e.LinkedStates, options =>
                    options.MapFrom(e => e.LinkedPlannedStates))
                .ForMember(e => e.LinkedPlannedStateIds, options =>
                    options.MapFrom(e => e.LinkedPlannedStates.Select(state => state.Id).ToList()));

            this.CreateMap<EventEntity, NewEvent>().ReverseMap();
            this.CreateMap<EventEntity, ModifiedEvent>()
                .ForMember(e => e.LinkedPlannedStateIds, options =>
                    options.MapFrom(e => e.LinkedPlannedStates.Select(state => state.Id).ToList()));
            this.CreateMap<ModifiedEvent, EventEntity>();

            this.CreateMap<Event, EventDto>().ReverseMap();
            this.CreateMap<Event, ManagerEventDto>().ReverseMap();

            this.CreateMap<EventWithLinkedStates, EventWithLinkedStatesDto>()
                .ForMember(e => e.LinkedStatesDtos, options =>
                    options.MapFrom(e => e.LinkedStates));
            this.CreateMap<EventWithLinkedStates, ManagerEventWithLinkedStatesDto>()
                .ForMember(e => e.LinkedStatesDtos, options =>
                    options.MapFrom(e => e.LinkedStates));

            this.CreateMap<NewEvent, BaseEventDto>().ReverseMap();
            this.CreateMap<ModifiedEvent, BaseEventDto>().ReverseMap();
        }
    }
}
