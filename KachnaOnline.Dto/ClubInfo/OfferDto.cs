// OfferDto.cs
// Author: Ondřej Ondryáš

namespace KachnaOnline.Dto.ClubInfo
{
    /// <summary>
    /// Represents a set of information about refreshments that are currently available to buy in the club.
    /// </summary>
    public class OfferDto
    {
        public OfferedItemDto[] Products { get; set; }
        public OfferedItemDto[] BeersOnTap { get; set; }
    }
}
