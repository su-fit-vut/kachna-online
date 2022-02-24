using System.Collections.Generic;
using KachnaOnline.Dto.Swagger;

namespace KachnaOnline.Dto.ClubInfo
{
    /// <summary>
    /// Represents a set of information about refreshments that are currently available to buy in the club.
    /// </summary>
    public class OfferDto
    {
        [SwaggerNotNull] public List<OfferedItemDto> Products { get; set; }
        [SwaggerNotNull] public List<OfferedItemDto> BeersOnTap { get; set; }
    }
}
