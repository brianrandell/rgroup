using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rg.ServiceCore.DbModel
{
    [Table("TimelineEntryMedia")]
    public class TimelineEntryMedia
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TimelineEntryMediaId { get; set; }

        public int Sequence { get; set; }

        [ForeignKey("TimelineEntry")]
        public int TimelineEntryId { get; set; }
        public virtual TimelineEntry TimelineEntry { get; set; }

        [ForeignKey("Media")]
        public int UserMediaId { get; set; }
        public virtual UserMedia Media { get; set; }
    }
}
