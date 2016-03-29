using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rg.ServiceCore.DbModel
{
    [Table("UserMedia")]
    public class UserMedia
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserMediaId { get; set; }

        public virtual UserMediaData Data { get; set; }

        [ForeignKey("User")]
        [Required]
        public string UserId { get; set; }
        public virtual UserInfo User { get; set; }

        [ForeignKey("MediaAlbum")]
        public int? MediaAlbumId { get; set; }
        public virtual MediaAlbum MediaAlbum { get; set; }

        [ForeignKey("CommentThread")]
        public int? CommentThreadId { get; set; }
        public virtual CommentThread CommentThread { get; set; }

        [ForeignKey("Description")]
        public int? DescriptionUserTextId { get; set; }
        public virtual UserText Description { get; set; }

        public DateTime CreatedUtc { get; set; }

        public UserMediaState State { get; set; }
        public string Title { get; set; }
        public string Extension { get; set; }

        public virtual IList<Like> Likes { get; set; }
    }
}
