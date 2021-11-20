// BoardGamesOptions.cs
// Author: František Nečas

namespace KachnaOnline.Business.Configuration
{
    public class BoardGamesOptions
    {
        /// <summary>
        /// The number of days that a reservation lasts unless the game specifies otherwise.
        /// </summary>
        public int DefaultReservationDays { get; set; }
        
        /// <summary>
        /// The number of days for which a reservation is extended.
        /// </summary>
        public int ExtensionDays { get; set; }
    }
}
