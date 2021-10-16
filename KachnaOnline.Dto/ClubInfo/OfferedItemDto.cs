// OfferedItemDto.cs
// Author: Ondřej Ondryáš

namespace KachnaOnline.Dto.ClubInfo
{
    /// <summary>
    /// Represents a kind of refreshment article one can buy in the club when the bar is open.
    /// </summary>
    public class OfferedItemDto
    {
        /// <summary>
        /// The name of the item.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The price of the item in Czech crowns.
        /// </summary>
        public int Price { get; set; }

        /// <summary>
        /// The amount of prestige points one gets when purchasing this item.
        /// </summary>
        public int Prestige { get; set; }

        /// <summary>
        /// A URL of an image of the item.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Signalizes whether the item is a permanent offer.
        /// </summary>
        public bool IsPermanentOffer { get; set; }
    }
}
