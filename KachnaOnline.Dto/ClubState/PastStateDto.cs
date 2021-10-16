// PastStateDto.cs
// Author: Ondřej Ondryáš

using System;
using System.Text.Json.Serialization;

namespace KachnaOnline.Dto.ClubState
{
    public class PastStateDto : StateDto
    {
        public DateTime ActualEnd { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public int? ClosedBy { get; set; }
    }
}
