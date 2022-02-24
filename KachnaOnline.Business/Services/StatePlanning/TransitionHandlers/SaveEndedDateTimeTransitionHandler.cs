using System;
using System.Threading.Tasks;
using KachnaOnline.Business.Data.Repositories.Abstractions;
using KachnaOnline.Business.Models.ClubStates;
using KachnaOnline.Business.Services.StatePlanning.Abstractions;

namespace KachnaOnline.Business.Services.StatePlanning.TransitionHandlers
{
    /// <summary>
    /// An <see cref="IStateTransitionHandler"/> that sets the state's Ended attribute in the database
    /// to the current datetime when the state ends.
    /// </summary>
    public class SaveEndedDateTimeTransitionHandler : IStateTransitionHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public SaveEndedDateTimeTransitionHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task PerformStartAction(int stateId, int? previousStateId)
        {
            return Task.CompletedTask;
        }

        public Task PerformModifyAction(State previousState)
        {
            return Task.CompletedTask;
        }

        public async Task PerformEndAction(int stateId, int? nextStateId)
        {
            var state = await _unitOfWork.PlannedStates.Get(stateId);
            if (!state.Ended.HasValue)
            {
                state.Ended = DateTime.Now;
                await _unitOfWork.SaveChanges();
            }
        }
    }
}
