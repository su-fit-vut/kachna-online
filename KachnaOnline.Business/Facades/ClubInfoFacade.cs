// ClubInfoFacade.cs
// Author: Ondřej Ondryáš

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using KachnaOnline.Business.Configuration;
using KachnaOnline.Business.Services.Abstractions;
using KachnaOnline.Dto.ClubInfo;
using Microsoft.Extensions.Options;

namespace KachnaOnline.Business.Facades
{
    public class ClubInfoFacade
    {
        private readonly IKisService _kisService;
        private readonly IOptionsMonitor<KisOptions> _kisOptionsMonitor;
        private readonly IMapper _mapper;

        public ClubInfoFacade(IKisService kisService, IOptionsMonitor<KisOptions> kisOptionsMonitor, IMapper mapper)
        {
            _kisService = kisService;
            _kisOptionsMonitor = kisOptionsMonitor;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns the current offer fetched from KIS.
        /// </summary>
        /// <returns>An <see cref="OfferDto"/> with non-null property values or null if an error occurs.</returns>
        public async Task<OfferDto> GetCurrentOffer()
        {
            var ret = new OfferDto();

            // Get taps/beers
            var taps = await _kisService.GetTapInfo();
            if (taps is { Count: >0 })
            {
                ret.BeersOnTap = taps
                    .Where(t => t.OfferedArticles is { Length: >0 })
                    .SelectMany(t => t.OfferedArticles)
                    .Select(a => _mapper.Map<OfferedItemDto>(a)).ToList();
            }

            // Get other items
            var items = await _kisService.GetOfferedArticles();
            if (items is { Count: >0 })
            {
                ret.Products = items
                    .Where(t => t.InStock is > 0 ||
                                t.Labels.Any(l => l.Id == _kisOptionsMonitor.CurrentValue.PermanentOfferLabelId))
                    .Select(a => _mapper.Map<OfferedItemDto>(a)).ToList();
            }

            // If both are null, there's probably a problem in the configuration (expired display token)
            // Signalize that to the user by returning null here (and sending a HTTP 500 from the controller)
            if (items == null && taps == null)
                return null;

            // If one of them is null, set to a blank list
            ret.Products ??= new List<OfferedItemDto>();
            ret.BeersOnTap ??= new List<OfferedItemDto>();

            return ret;
        }

        /// <summary>
        /// Returns today's prestige leaderboard.
        /// </summary>
        /// <returns>A list of <see cref="LeaderboardItemDto"/> or null if an error occurs.
        /// The list is in ascending order – the first element denotes the person with the most prestige points.</returns>
        public async Task<List<LeaderboardItemDto>> GetTodayLeaderboard()
        {
            var from = DateTime.Today;
            var to = DateTime.Today.Add(new TimeSpan(23, 59, 59));

            var leaderboard =
                await _kisService.GetLeaderboard(from, to, _kisOptionsMonitor.CurrentValue.NumberOfLeaderboardItems);
            if (leaderboard is null)
                return null;

            return leaderboard.Select(l => _mapper.Map<LeaderboardItemDto>(l))
                .ToList();
        }

        /// <summary>
        /// Returns the current semester prestige leaderboard.
        /// </summary>
        /// <remarks>
        /// A 'semester' is actually one of three periods: 1 Sep to 31 Jan (winter semester), 1 Feb to 31 May (summer
        /// semester) or 1 June to 31 Aug (summer holiday).
        /// </remarks>
        /// <returns>A list of <see cref="LeaderboardItemDto"/> or null if an error occurs.
        /// The list is in ascending order – the first element denotes the person with the most prestige points.</returns>
        public async Task<List<LeaderboardItemDto>> GetSemesterLeaderboard()
        {
            var now = DateTime.Now;
            var from = now.Month switch
            {
                >= 9 => new DateTime(now.Year, 9, 1), // Sep to Dec => Winter sem.
                1 => new DateTime(now.Year - 1, 9, 1), // Jan => Still winter sem.
                >1 and <6 => new DateTime(now.Year, 2, 1), // Feb to May => Summer sem.
                _ => new DateTime(now.Year, 6, 1) // June to Aug => Summer holidays
            };
            var to = now.Month switch
            {
                >= 9 => new DateTime(now.Year + 1, 1, 31), // Sep to Dec => Winter sem.
                1 => new DateTime(now.Year, 1, 31), // Jan => Still winter sem.
                >1 and <6 => new DateTime(now.Year, 5, 31), // Feb to May => Summer sem.
                _ => new DateTime(now.Year, 8, 31) // June to Aug => Summer holidays
            };

            from = from.ToLocalTime();
            to = to.ToLocalTime();

            var leaderboard =
                await _kisService.GetLeaderboard(from, to, _kisOptionsMonitor.CurrentValue.NumberOfLeaderboardItems);
            if (leaderboard is null)
                return null;

            return leaderboard.Select(l => _mapper.Map<LeaderboardItemDto>(l))
                .ToList();
        }
    }
}
