// EventsService.cs
// Author: David Chocholatý

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KachnaOnline.Business.Constants;
using KachnaOnline.Business.Data.Repositories.Abstractions;
using KachnaOnline.Business.Exceptions;
using KachnaOnline.Business.Exceptions.Events;
using KachnaOnline.Business.Extensions;
using KachnaOnline.Business.Services.Abstractions;
using KachnaOnline.Business.Models.Events;
using Microsoft.Extensions.Logging;
using EventEntity = KachnaOnline.Data.Entities.Events.Event;

namespace KachnaOnline.Business.Services
{
    public class EventsService : IEventsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<EventsService> _logger;
        private readonly IEventsRepository _eventsRepository;
        private readonly IUserRepository _userRepository;

        private const int EventPlanningEnterTimeout = 1000;
        private static readonly SemaphoreSlim EventPlanningSemaphore = new(1);

        private static async Task EnsureLock()
        {
            var hasLock = await EventPlanningSemaphore.WaitAsync(EventPlanningEnterTimeout);
            if (!hasLock)
                throw new EventManipulationFailedException("Cannot acquire the event planning lock.");
        }

        public EventsService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<EventsService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _eventsRepository = _unitOfWork.Events;
            _userRepository = _unitOfWork.Users;
        }

        /// <inheritdoc />
        public async Task<ICollection<Event>> GetCurrentEvents()
        {
            var eventEntities = _eventsRepository.GetCurrent();
            var resultList = new List<Event>();
            await foreach (var eventEntity in eventEntities)
            {
                resultList.Add(_mapper.Map<Event>(eventEntity));
            }

            return resultList;
        }

        /// <inheritdoc />
        public async Task<Event> GetEvent(int eventId)
        {
            var eventEntity = await _eventsRepository.Get(eventId);
            if (eventEntity is null)
                throw new EventNotFoundException();

            return _mapper.Map<Event>(eventEntity);
        }

        /// <inheritdoc />
        public async Task<ICollection<Event>> GetNextPlannedEvents()
        {
            var eventEntities = _eventsRepository.GetNearest();
            var resultList = new List<Event>();

            try
            {
                await foreach (var eventEntity in eventEntities)
                {
                    resultList.Add(_mapper.Map<Event>(eventEntity));
                }
            }
            catch (InvalidOperationException)
            {
            }

            return resultList;
        }

        /// <inheritdoc />
        public async Task<ICollection<Event>> GetEvents(DateTime? from = null, DateTime? to = null)
        {
            from ??= DateTime.Now;
            to ??= from.Value.AddDays(EventConstants.QueryDaysTimeSpan);

            if (to < from)
                throw new ArgumentException(
                    $"The {nameof(to)} argument must not be a datetime before {nameof(from)}.");

            if (to - from > TimeSpan.FromDays(EventConstants.QueryDaysTimeSpan))
                throw new ArgumentException(
                    $"The maximum time span to get events for is {EventConstants.QueryDaysTimeSpan} days.");

            var eventEntities = _eventsRepository.GetStartingBetween(
                from.Value.RoundToMinutes(), to.Value.RoundToMinutes());

            var resultList = new List<Event>();
            await foreach (var eventEntity in eventEntities)
            {
                resultList.Add(_mapper.Map<Event>(eventEntity));
            }

            return resultList;
        }

        /// <inheritdoc />
        public async Task<ICollection<Event>> GetEvents(DateTime at)
        {
            var eventEntities = _eventsRepository.GetCurrent(at);
            var resultList = new List<Event>();
            await foreach (var eventEntity in eventEntities)
            {
                resultList.Add(_mapper.Map<Event>(eventEntity));
            }

            return resultList;
        }

        /// <inheritdoc />
        public async Task<Event> PlanEvent(NewEvent newEvent)
        {
            if (newEvent is null)
                throw new ArgumentNullException(nameof(newEvent));

            newEvent.From = newEvent.From.RoundToMinutes();
            if (newEvent.From < DateTime.Now.RoundToMinutes())
                throw new ArgumentException("Cannot plan an event in the past.", nameof(newEvent));

            newEvent.To = newEvent.To.RoundToMinutes();
            if (newEvent.To < newEvent.From)
                throw new ArgumentException("Cannot plan an event which ends before it starts.", nameof(newEvent));

            if (await _userRepository.Get(newEvent.MadeById) is null)
                throw new UserNotFoundException(newEvent.MadeById);

            var newEventEntity = _mapper.Map<EventEntity>(newEvent);

            await EnsureLock();

            await _eventsRepository.Add(newEventEntity);

            try
            {
                await _unitOfWork.SaveChanges();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Cannot save a new event.");
                await _unitOfWork.ClearTrackedChanges();
                throw new EventManipulationFailedException();
            }
            finally
            {
                EventPlanningSemaphore.Release();
            }

            // TODO Get new event entity and conflicting states.
            return _mapper.Map<Event>(newEventEntity);
        }

        /// <inheritdoc />
        public async Task ModifyEvent(int eventId, ModifiedEvent modifiedEvent)
        {
            if (modifiedEvent is null)
                throw new ArgumentNullException(nameof(modifiedEvent));

            var eventEntity = await _eventsRepository.Get(eventId);
            if (eventEntity is null)
                throw new EventNotFoundException();

            if (eventEntity.To < DateTime.Now.RoundToMinutes())
                throw new EventReadOnlyException();

            await EnsureLock();

            _mapper.Map<ModifiedEvent, EventEntity>(modifiedEvent, eventEntity);

            try
            {
                await _unitOfWork.SaveChanges();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Cannot modify the event with ID {eventId}.");
                await _unitOfWork.ClearTrackedChanges();
                throw new EventManipulationFailedException();
            }
            finally
            {
                EventPlanningSemaphore.Release();
            }
        }

        /// <inheritdoc />
        public async Task RemoveEvent(int eventId)
        {
            var eventEntity = await _eventsRepository.Get(eventId);
            if (eventEntity is null)
                throw new EventNotFoundException();

            if (eventEntity.To < DateTime.Now.RoundToMinutes())
                throw new EventReadOnlyException();

            await EnsureLock();

            await _eventsRepository.Delete(eventEntity);

            try
            {
                await _unitOfWork.SaveChanges();
                // TODO Notify linked states.
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Cannot remove the event with ID {eventId}.");
                await _unitOfWork.ClearTrackedChanges();
                Console.WriteLine(exception.Message);
                throw new EventManipulationFailedException();
            }
            finally
            {
                EventPlanningSemaphore.Release();
            }
        }
    }
}
