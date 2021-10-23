// DateTimeExtensions.cs
// Author: Ondřej Ondryáš

using System;

namespace KachnaOnline.Business.Extensions
{
    public static class TimeSpanExtensions
    {
        /// <summary>
        /// Creates a new <see cref="TimeSpan"/> that only keeps the hour and minute components
        /// from the original <paramref name="timeSpan"/>.
        /// </summary>
        /// <param name="timeSpan">The <see cref="TimeSpan"/> to round.</param>
        /// <returns>A <see cref="TimeSpan"/> structure with the date, hour and minute components set.</returns>
        public static TimeSpan RoundToMinutes(this TimeSpan timeSpan)
        {
            return new TimeSpan(timeSpan.Hours, timeSpan.Minutes, 0);
        }
    }
}
