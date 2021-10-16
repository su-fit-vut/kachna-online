// OfferDto.cs
// Author: Ondřej Ondryáš

namespace KachnaOnline.Dto.ClubInfo
{
    public class OfferDto
    {
        public OfferedItemDto[] Products { get; set; }
        public OfferedItemDto[] BeersOnTap { get; set; }
    }
}
