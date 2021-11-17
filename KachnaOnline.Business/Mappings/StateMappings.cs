// StateMappings.cs
// Author: Ondřej Ondryáš

using AutoMapper;
using KachnaOnline.Business.Models.ClubStates;
using KachnaOnline.Data.Entities.ClubStates;
using KachnaOnline.Dto.ClubStates;
using RepeatingState = KachnaOnline.Business.Models.ClubStates.RepeatingState;

namespace KachnaOnline.Business.Mappings
{
    public class StateMappings : Profile
    {
        public StateMappings()
        {
            this.CreateMap<PlannedState, State>()
                .ForMember(m => m.FollowingState, options =>
                    options.MapFrom(e => e.NextPlannedState))
                .ForMember(m => m.EventId, options =>
                    options.MapFrom(e => e.AssociatedEventId))
                .ForMember(m => m.Type, options =>
                    options.MapFrom(e => e.State));

            this.CreateMap<KachnaOnline.Data.Entities.ClubStates.RepeatingState, RepeatingState>();

            this.CreateMap<RepeatingState, KachnaOnline.Data.Entities.ClubStates.RepeatingState>()
                .ForMember(m => m.Id, options =>
                    options.Ignore());

            this.CreateMap<State, StateDto>()
                .ForMember(dto => dto.State, options =>
                    options.MapFrom(m => m.Type))
                .ForMember(dto => dto.MadeBy, options =>
                    options.Ignore())
                .ForMember(dto => dto.Note, options =>
                    options.MapFrom(m => m.NotePublic));
            
            this.CreateMap<State, PastStateDto>()
                .ForMember(dto => dto.State, options =>
                    options.MapFrom(m => m.Type))
                .ForMember(dto => dto.MadeBy, options =>
                    options.Ignore())
                .ForMember(dto => dto.Note, options =>
                    options.MapFrom(m => m.NotePublic))
                .ForMember(dto => dto.ActualEnd, options =>
                    options.MapFrom(m => m.Ended))
                .ForMember(dto => dto.ClosedBy, options =>
                    options.Ignore());

            this.CreateMap<StatePlanningDto, NewState>()
                .ForMember(m => m.Type, options =>
                    options.MapFrom(dto => dto.State));

            this.CreateMap<StateModificationDto, StateModification>();
        }
    }
}
