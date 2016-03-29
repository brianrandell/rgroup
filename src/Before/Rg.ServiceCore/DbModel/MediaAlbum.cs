using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rg.ServiceCore.DbModel
{
    [Table("MediaAlbum")]
    public class MediaAlbum
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MediaAlbumId { get; set; }

        [ForeignKey("User")]
        [Required]
        public string UserId { get; set; }
        public virtual UserInfo User { get; set; }

        [MaxLength(200)]
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public virtual IList<Like> Likes { get; set; }

        public virtual IList<UserMedia> UserMedias { get; set; }

        [ForeignKey("CommentThread")]
        public int? CommentThreadId { get; set; }
        public virtual CommentThread CommentThread { get; set; }
    }
}
