using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rg.ServiceCore.DbModel
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// The timestamp for this is implied by the <see cref="Message"/>.
    /// </remarks>
    [Table("TimelineEntry")]
    public class TimelineEntry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TimelineEntryId { get; set; }

        [ForeignKey("User")]
        [Required]
        public string UserId { get; set; }
        public virtual UserInfo User { get; set; }

        [ForeignKey("Message")]
        [Required]
        public int MessageUserTextId { get; set; }
        public virtual UserText Message { get; set; }

        [ForeignKey("CommentThread")]
        public int? CommentThreadId { get; set; }
        public virtual CommentThread CommentThread { get; set; }

        public virtual IList<TimelineEntryMedia> Media { get; set; }
    }
}
