using System;

namespace KachnaOnline.Business.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Creates a new <see cref="DateTime"/> that only keeps the date, hour and minute components
        /// from the original <paramref name="dateTime"/>.
        /// </summary>
        /// <param name="dateTime">The <see cref="DateTime"/> to round.</param>
        /// <returns>A <see cref="DateTime"/> structure with the date, hour and minute components set.</returns>
        public static DateTime RoundToMinutes(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day,
                dateTime.Hour, dateTime.Minute, 0);
        }
    }
}
