using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rg.ServiceCore.DbModel
{
    [Table("Comment")]
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CommentId { get; set; }

        [ForeignKey("Text")]
        public int UserTextId { get; set; }
        public virtual UserText Text { get; set; }

        [ForeignKey("User")]
        [Required]
        public string UserId { get; set; }
        public virtual UserInfo User { get; set; }

        [ForeignKey("Thread")]
        public int CommentThreadId { get; set; }
        public virtual CommentThread Thread { get; set; }
    }
}
