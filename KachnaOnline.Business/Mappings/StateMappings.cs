// StateMappings.cs
// Author: Ondřej Ondryáš

using System;
using AutoMapper;
using KachnaOnline.Business.Models.ClubStates;
using KachnaOnline.Data.Entities.ClubStates;
using KachnaOnline.Dto.ClubStates;
using RepeatingState = KachnaOnline.Business.Models.ClubStates.RepeatingState;

namespace KachnaOnline.Business.Mappings
{
    public class StateModelFollowingStateValueResolver : IValueResolver<PlannedState, State, State>
    {
        public State Resolve(PlannedState source, State destination, State destMember, ResolutionContext context)
        {
            if (source.NextPlannedState != null)
            {
                return context.Mapper.Map<State>(source.NextPlannedState);
            }

            if (source.NextPlannedStateId.HasValue)
            {
                return new State() {Id = source.NextPlannedStateId.Value};
            }

            return null;
        }
    }

    public class StateDtoFollowingStateValueResolver : IValueResolver<State, StateDto, StateStubDto>
    {
        public StateStubDto Resolve(State source, StateDto destination, StateStubDto destMember,
            ResolutionContext context)
        {
            if (source.FollowingState == null)
            {
                return null;
            }
            
            if (source.FollowingState.Start == default)
            {
                return new StateStubDto() {Id = source.FollowingState.Id};
            }

            return context.Mapper.Map<StateDto>(source.FollowingState);
        }
    }

    public class StateMappings : Profile
    {
        public StateMappings()
        {
            this.CreateMap<PlannedState, State>()
                .ForMember(m => m.FollowingState, options =>
                    options.MapFrom<StateModelFollowingStateValueResolver>())
                .ForMember(m => m.EventId, options =>
                    options.MapFrom(e => e.AssociatedEventId))
                .ForMember(m => m.Type, options =>
                    options.MapFrom(e => e.State));

            this.CreateMap<KachnaOnline.Data.Entities.ClubStates.RepeatingState, RepeatingState>();

            this.CreateMap<RepeatingState, KachnaOnline.Data.Entities.ClubStates.RepeatingState>()
                .ForMember(m => m.Id, options =>
                    options.Ignore());

            this.CreateMap<RepeatingState, RepeatingStateDto>()
                .ForMember(dto => dto.Note, options =>
                    options.MapFrom(m => m.NotePublic));

            this.CreateMap<RepeatingState, RepeatingStateManagerDto>()
                .IncludeBase<RepeatingState, RepeatingStateDto>()
                .ForMember(dto => dto.MadeBy, options =>
                    options.Ignore());

            this.CreateMap<RepeatingStatePlanningDto, RepeatingState>();

            this.CreateMap<State, StateDto>()
                .ForMember(dto => dto.State, options =>
                    options.MapFrom(m => m.Type))
                .ForMember(dto => dto.MadeBy, options =>
                    options.Ignore())
                .ForMember(dto => dto.Note, options =>
                    options.MapFrom(m => m.NotePublic))
                .ForMember(dto => dto.FollowingState, options =>
                    options.MapFrom<StateDtoFollowingStateValueResolver>());

            this.CreateMap<State, PastStateDto>()
                .IncludeBase<State, StateDto>()
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
