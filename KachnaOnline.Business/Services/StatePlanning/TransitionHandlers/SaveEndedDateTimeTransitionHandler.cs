// SaveEndedDateTimeTransitionHandler.cs
// Author: Ondřej Ondryáš

using System;
using System.Threading.Tasks;
using KachnaOnline.Business.Data.Repositories.Abstractions;
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

        public Task PerformStartAction(int stateId)
        {
            return Task.CompletedTask;
        }

        public async Task PerformEndAction(int stateId)
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
