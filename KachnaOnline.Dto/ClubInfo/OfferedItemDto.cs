// OfferedItemDto.cs
// Author: Ondřej Ondryáš

namespace KachnaOnline.Dto.ClubInfo
{
    public class OfferedItemDto
    {
        public string Name { get;set; }
        public int Price { get; set; }
        public int Prestige { get; set; }
        public string ImageUrl { get; set; } 
        public bool IsPermanentOffer { get; set; }
    }
}
