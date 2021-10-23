// StateMappings.cs
// Author: Ondřej Ondryáš

using AutoMapper;
using KachnaOnline.Business.Models.ClubStates;
using KachnaOnline.Data.Entities.ClubStates;
using RepeatingState = KachnaOnline.Business.Models.ClubStates.RepeatingState;

namespace KachnaOnline.Business.Mappings
{
    public class StateMappings : Profile
    {
        public StateMappings()
        {
            this.CreateMap<PlannedState, State>()
                .ForMember(m => m.FollowingState, options =>
                    options.MapFrom(e => e.NextPlannedState));

            this.CreateMap<KachnaOnline.Data.Entities.ClubStates.RepeatingState, RepeatingState>();

            this.CreateMap<RepeatingState, KachnaOnline.Data.Entities.ClubStates.RepeatingState>()
                .ForMember(m => m.Id, options => options.Ignore());
        }
    }
}
