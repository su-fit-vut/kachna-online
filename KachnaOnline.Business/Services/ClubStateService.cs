// ClubStateService.cs
// Author: Ondřej Ondryáš

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KachnaOnline.Business.Models.ClubStates;
using KachnaOnline.Business.Services.Abstractions;

namespace KachnaOnline.Business.Services
{
    public class ClubStateService : IClubStateService
    {
        public Task<State> GetCurrentState()
        {
            throw new NotImplementedException();
        }

        public Task<State> GetState(int id)
        {
            throw new NotImplementedException();
        }

        public Task<State> GetNextPlannedState(StateType type)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<State>> GetStates(DateTime @from, DateTime to)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<RepeatingState>> GetRepeatingStates(DateTime? effectiveFrom = null, DateTime? effectiveTo = null)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<RepeatingState>> GetRepeatingStates(DateTime effectiveAt)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<State>> GetStatesForRepeatingState(int repeatingStateId, bool futureOnly = true)
        {
            throw new NotImplementedException();
        }

        public Task MakeRepeatingState(RepeatingState newRepeatingState)
        {
            throw new NotImplementedException();
        }

        public Task RemoveRepeatingState(int repeatingStateId)
        {
            throw new NotImplementedException();
        }

        public Task ModifyRepeatingState(RepeatingState repeatingState, int changeMadeByUserId)
        {
            throw new NotImplementedException();
        }

        public Task PlanState(State newState)
        {
            throw new NotImplementedException();
        }

        public Task RemovePlannedState(int id)
        {
            throw new NotImplementedException();
        }

        public Task ModifyState(StateModification stateModification, int changeMadeByUserId)
        {
            throw new NotImplementedException();
        }

        public Task CloseNow(int closedByUserId)
        {
            throw new NotImplementedException();
        }
    }
}
