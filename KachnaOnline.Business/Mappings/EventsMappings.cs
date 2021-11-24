// EventsMappings.cs
// Author: David Chocholatý

using AutoMapper;
using KachnaOnline.Business.Models.Events;
using KachnaOnline.Dto.Events;
using EventEntity = KachnaOnline.Data.Entities.Events.Event;

namespace KachnaOnline.Business.Mappings
{
    public class EventMappings : Profile
    {
        public EventMappings()
        {
            this.CreateMap<EventEntity, Event>().ReverseMap();
            this.CreateMap<EventEntity, NewEvent>().ReverseMap();
            this.CreateMap<EventEntity, ModifiedEvent>().ReverseMap();
            this.CreateMap<Event, EventDto>().ReverseMap();
            this.CreateMap<Event, ManagerEventDto>().ReverseMap();
            this.CreateMap<NewEvent, BaseEventDto>().ReverseMap();
            this.CreateMap<ModifiedEvent, BaseEventDto>().ReverseMap();
        }
    }
}
