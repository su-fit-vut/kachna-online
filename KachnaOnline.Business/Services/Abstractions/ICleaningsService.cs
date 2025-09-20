using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KachnaOnline.Business.Exceptions;
using KachnaOnline.Business.Exceptions.Cleanings;
using KachnaOnline.Business.Models.Cleanings;

namespace KachnaOnline.Business.Services.Abstractions
{
    public interface ICleaningsService
    {
        /// <summary>
        /// Returns a collection of current cleanings.
        /// </summary>
        /// <returns>A Collection of <see cref="Cleaning"/> of current cleanings or an empty collection if no there is no cleaning
        /// held currently. The collection usually contains maximally one cleaning, more in case multiple next cleanings start
        /// at the same time.</returns>
        Task<ICollection<Cleaning>> GetCurrentCleanings();

        /// <summary>
        /// Returns the cleaning corresponding to the specified <paramref name="cleaningId"/>.
        /// </summary>
        /// <param name="cleaningId">The cleaning ID to search for.</param>
        /// <returns>An <see cref="Cleaning"/> object containing the cleaning matching the specified <paramref name="cleaningId"/>
        /// </returns>
        /// <exception cref="CleaningNotFoundException"> thrown when a cleaning with the given <paramref name="cleaningId"/>
        /// does not exist.</exception>
        Task<Cleaning> GetCleaning(int cleaningId);

        /// <summary>
        /// Returns a list of next (closest to now) planned cleanings.
        /// </summary>
        /// <remarks>Usually returns only one cleaning, only if two or more next cleanings start at the same time,
        /// list contains more than one cleaning.</remarks>
        /// <returns>An <see cref="Cleaning"/> object containing the next cleaning if such cleaning exists,
        /// or null if it doesn't.</returns>
        Task<ICollection<Cleaning>> GetNextPlannedCleanings();

        /// <summary>
        /// Returns a list of cleanings starting in the future, optionally only the ones starting in the timespan specified by <paramref name="from"/> and <paramref name="to"/>.
        /// </summary>
        /// <param name="from">If not null, only returns cleanings with their from date being after or equal to the specified value.</param>
        /// <param name="to">If not null, only returns cleanings with their to date being before or equal to the specified value.</param>
        /// <returns>A Collection of <see cref="Cleaning"/> or an empty list if no there is no cleaning planned.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="from"/> or <paramref name="to"/> are invalid.</exception>
        Task<ICollection<Cleaning>> GetCleanings(DateTime? from = null, DateTime? to = null);

        /// <summary>
        /// Returns a collection of all cleanings that are held at the date and time specified by <paramref name="at"/>.
        /// </summary>
        /// <param name="at">Date and time to look for held cleanings.</param>
        /// <returns>A Collection of <see cref="Cleaning"/> or an empty list if there is no cleaning at the specified date and time.</returns>
        Task<ICollection<Cleaning>> GetCleanings(DateTime at);

        /// <summary>
        /// Plans a new cleaning.
        /// </summary>
        /// <remarks>
        /// A cleaning can be planned even if it begins earlier than or in the middle of an another already planned cleaning that it would overlap.
        /// </remarks>
        /// <param name="newCleaning"><see cref="NewCleaning"/> to plan.</param>
        /// <returns> The planned <see cref="Cleaning"/> with a filled ID.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="newCleaning"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="newCleaning"/> has invalid attributes.</exception>
        /// <exception cref="CleaningManipulationFailedException">Thrown when the board game cannot be created.
        /// This can be caused by a database error.</exception>
        /// <exception cref="UserNotFoundException">Thrown when a user with the ID assigned to the cleaning does
        /// not exist.</exception>
        Task<Cleaning> PlanCleaning(NewCleaning newCleaning);

        /// <summary>
        /// Removes a cleaning record with the ID <paramref name="cleaningId"/>.
        /// </summary>
        /// <param name="cleaningId">ID of the cleaning to remove.</param>
        /// <exception cref="CleaningNotFoundException">Thrown when the cleaning with the given <paramref name="cleaningId"/> does not
        /// exist.</exception>
        /// <exception cref="CleaningManipulationFailedException">Thrown when the cleaning cannot be deleted.</exception>
        /// <exception cref="CleaningReadOnlyException">Thrown when cleaning to be removed has already ended.</exception>
        Task RemoveCleaning(int cleaningId);

                /// <summary>
        /// Joins a cleaning record with the ID <paramref name="cleaningId"/>.
        /// </summary>
        /// <param name="cleaningId">ID of the cleaning to join.</param>
        /// <exception cref="CleaningNotFoundException">Thrown when the cleaning with the given <paramref name="cleaningId"/> does not
        /// exist.</exception>
        /// <exception cref="CleaningManipulationFailedException">Thrown when the cleaning cannot be joined.</exception>
        /// <exception cref="CleaningReadOnlyException">Thrown when cleaning to be joined has already ended.</exception>
        Task JoinCleaning(int cleaningId);

        /// <summary>
        /// Changes details of a cleaning specified by <paramref name="cleaningId"/>. Projects these changes into all planned states linked to this cleaning. State records from the past are not changed.
        /// </summary>
        /// <remarks>
        /// When one of <see cref="Cleaning.From"/> or <see cref="Cleaning.To"/>
        /// is changed, new states may be planned or existing planned states may be unlinked and/or removed. These properties
        /// cannot be changed if the dates have already passed.<br/>
        /// <see cref="Cleaning.To"/> cannot be changed to a date in the past.<br/>
        /// Cleanings of which the <see cref="Cleaning.To"/>
        /// has already passed cannot be modified at all.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when the passed <paramref name="modifiedCleaning"/> model is null.</exception>
        /// <exception cref="CleaningNotFoundException">Thrown when a cleaning with the given <paramref name="cleaningId"/>
        /// does not exist.</exception>
        /// <exception cref="CleaningReadOnlyException">Thrown when cleaning to be modified has already ended.</exception>
        /// <exception cref="CleaningManipulationFailedException">Thrown when cleaning modification has failed.</exception>
        Task ModifyCleaning(int cleaningId, ModifiedCleaning modifiedCleaning);
    }
}
