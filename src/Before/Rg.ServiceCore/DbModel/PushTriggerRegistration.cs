using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rg.ServiceCore.DbModel
{
    [Table("PushTriggerRegistration")]
    public class PushTriggerRegistration
    {
        [Key, Column(Order = 0)]
        public TriggerType TypeId { get; set; }

        [Key, Column(Order = 1)]
        public string TriggerId { get; set; }

        public string CallbackUrl { get; set; }
    }
}
