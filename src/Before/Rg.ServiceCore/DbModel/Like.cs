using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rg.ServiceCore.DbModel
{
    [Table("Like")]
    public class Like
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LikeId { get; set; }

        [ForeignKey("User")]
        [Required]
        public string UserId { get; set; }
        public virtual UserInfo User { get; set; }

        [ForeignKey("UserText")]
        public int? UserTextId { get; set; }
        public virtual UserText UserText { get; set; }

        [ForeignKey("UserMedia")]
        public int? UserMediaId { get; set; }
        public virtual UserMedia UserMedia { get; set; }

        [ForeignKey("MediaAlbum")]
        public int? MediaAlbumId { get; set; }
        public virtual MediaAlbum MediaAlbum { get; set; }

        public LikeKind Kind { get; set; }
    }
}
