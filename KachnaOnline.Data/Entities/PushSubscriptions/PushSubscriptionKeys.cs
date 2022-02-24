using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KachnaOnline.Data.Entities.PushSubscriptions
{
    [Table("PushSubscriptionKey")]
    public class PushSubscriptionKey
    {
        [Required] public string Endpoint { get; set; }
        [Required] public string KeyType { get; set; }
        [Required] public string KeyValue { get; set; }
    }
}
