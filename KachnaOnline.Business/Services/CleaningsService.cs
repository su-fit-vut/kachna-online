using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using KachnaOnline.Business.Configuration;
using KachnaOnline.Business.Data.Repositories.Abstractions;
using KachnaOnline.Business.Exceptions;
using KachnaOnline.Business.Exceptions.Cleanings;
using KachnaOnline.Business.Extensions;
using KachnaOnline.Business.Services.Abstractions;
using KachnaOnline.Business.Models.Cleanings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using CleaningEntity = KachnaOnline.Data.Entities.Cleanings.Cleaning;

namespace KachnaOnline.Business.Services
{
    public class CleaningsService : ICleaningsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CleaningsService> _logger;
        private readonly ICleaningsRepository _cleaningsRepository;
        private readonly IUserRepository _userRepository;
        private readonly IOptionsMonitor<CleaningsOptions> _cleaningsOptionsMonitor;

        public CleaningsService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CleaningsService> logger,
            IOptionsMonitor<CleaningsOptions> cleaningsOptionsMonitor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _cleaningsRepository = _unitOfWork.Cleanings;
            _userRepository = _unitOfWork.Users;
            _cleaningsOptionsMonitor = cleaningsOptionsMonitor;
        }

        /// <inheritdoc />
        public async Task<ICollection<Cleaning>> GetCurrentCleanings()
        {
            var cleaningEntities = _cleaningsRepository.GetCurrent();
            var resultList = new List<Cleaning>();
            await foreach (var cleaningEntity in cleaningEntities)
            {
                resultList.Add(_mapper.Map<Cleaning>(cleaningEntity));
            }

            return resultList;
        }

        /// <inheritdoc />
        public async Task<Cleaning> GetCleaning(int cleaningId)
        {
            var cleaningEntity = await _cleaningsRepository.Get(cleaningId);

            if (cleaningEntity is null)
                throw new CleaningNotFoundException();

            return _mapper.Map<Cleaning>(cleaningEntity);
        }

        /// <inheritdoc />
        public async Task<ICollection<Cleaning>> GetNextPlannedCleanings()
        {
            var cleaningEntities = _cleaningsRepository.GetNearest();
            var resultList = new List<Cleaning>();

            try
            {
                await foreach (var cleaningEntity in cleaningEntities)
                {
                    resultList.Add(_mapper.Map<Cleaning>(cleaningEntity));
                }
            }
            catch (InvalidOperationException)
            {
            }

            return resultList;
        }

        /// <inheritdoc />
        public async Task<ICollection<Cleaning>> GetCleanings(DateTime? from = null, DateTime? to = null)
        {
            from ??= DateTime.Now;
            to ??= from.Value.AddDays(_cleaningsOptionsMonitor.CurrentValue.QueryDaysTimeSpan);

            if (to < from)
            {
                throw new ArgumentException(
                    $"The {nameof(to)} argument must not be a datetime before {nameof(@from)}.");
            }

            if (to - from > TimeSpan.FromDays(_cleaningsOptionsMonitor.CurrentValue.QueryDaysTimeSpan))
            {
                throw new ArgumentException(
                    $"The maximum time span to get cleanings for is {_cleaningsOptionsMonitor.CurrentValue.QueryDaysTimeSpan} days.");
            }

            var cleaningEntities = _cleaningsRepository.GetStartingBetween(
                from.Value.RoundToMinutes(), to.Value.RoundToMinutes());

            var resultList = new List<Cleaning>();
            await foreach (var cleaningEntity in cleaningEntities)
            {
                resultList.Add(_mapper.Map<Cleaning>(cleaningEntity));
            }

            return resultList;
        }

        /// <inheritdoc />
        public async Task<ICollection<Cleaning>> GetCleanings(DateTime at)
        {
            var cleaningEntities = _cleaningsRepository.GetCurrent(at);
            var resultList = new List<Cleaning>();
            await foreach (var cleaningEntity in cleaningEntities)
            {
                resultList.Add(_mapper.Map<Cleaning>(cleaningEntity));
            }

            return resultList;
        }

        /// <inheritdoc />
        public async Task<Cleaning> PlanCleaning(NewCleaning newCleaning)
        {
            if (newCleaning is null)
                throw new ArgumentNullException(nameof(newCleaning));

            newCleaning.From = newCleaning.From.RoundToMinutes();
            if (newCleaning.From < DateTime.Now.RoundToMinutes())
                throw new ArgumentException("Cannot plan a cleaning in the past.", nameof(newCleaning));

            newCleaning.To = newCleaning.To.RoundToMinutes();
            if (newCleaning.To < newCleaning.From)
                throw new ArgumentException("Cannot plan a cleaning which ends before it starts.", nameof(newCleaning));

            if (await _userRepository.Get(newCleaning.MadeById) is null)
                throw new UserNotFoundException(newCleaning.MadeById);

            var newCleaningEntity = _mapper.Map<CleaningEntity>(newCleaning);

            await _cleaningsRepository.Add(newCleaningEntity);

            try
            {
                await _unitOfWork.SaveChanges();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Cannot save a new cleaning.");
                await _unitOfWork.ClearTrackedChanges();
                throw new CleaningManipulationFailedException();
            }

            return _mapper.Map<Cleaning>(newCleaningEntity);
        }

        /// <inheritdoc />
        public async Task ModifyCleaning(int cleaningId, ModifiedCleaning modifiedCleaning)
        {
            if (modifiedCleaning is null)
                throw new ArgumentNullException(nameof(modifiedCleaning));

            var cleaningEntity = await _cleaningsRepository.Get(cleaningId);
            if (cleaningEntity is null)
                throw new CleaningNotFoundException();

            if (cleaningEntity.To < DateTime.Now.RoundToMinutes())
                throw new CleaningReadOnlyException();

            _mapper.Map(modifiedCleaning, cleaningEntity);
            try
            {
                await _unitOfWork.SaveChanges();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Cannot modify the cleaning with ID {cleaningId}.");
                await _unitOfWork.ClearTrackedChanges();
                throw new CleaningManipulationFailedException();
            }
        }

        /// <inheritdoc />
        public async Task RemoveCleaning(int cleaningId)
        {
            var cleaningEntity = await _cleaningsRepository.Get(cleaningId);
            if (cleaningEntity is null)
                throw new CleaningNotFoundException();

            if (cleaningEntity.To < DateTime.Now.RoundToMinutes())
                throw new CleaningReadOnlyException();

            // Remove cleaning, set all linked planned states references to this cleaning to null.
            await _cleaningsRepository.Delete(cleaningEntity);
            try
            {
                await _unitOfWork.SaveChanges();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Cannot remove the cleaning with ID {cleaningId}.");
                await _unitOfWork.ClearTrackedChanges();
                throw new CleaningManipulationFailedException();
            }
        }
        
        public async Task JoinCleaning(int cleaningId)
        {
            var cleaningEntity = await _cleaningsRepository.Get(cleaningId);
            if (cleaningEntity is null)
                throw new CleaningNotFoundException();

            if (cleaningEntity.To < DateTime.Now.RoundToMinutes())
                throw new CleaningReadOnlyException();

            // Remove cleaning, set all linked planned states references to this cleaning to null.
            await _cleaningsRepository.Delete(cleaningEntity);
            try
            {
                await _unitOfWork.SaveChanges();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Cannot remove the cleaning with ID {cleaningId}.");
                await _unitOfWork.ClearTrackedChanges();
                throw new CleaningManipulationFailedException();
            }
        }
    }
}
