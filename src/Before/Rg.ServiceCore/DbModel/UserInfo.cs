using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rg.ServiceCore.DbModel
{
    [Table("UserInfo")]
    public class UserInfo
    {
        [Key]
        [MaxLength(36)]
        [Column(TypeName = "varchar")]
        public string UserInfoId { get; set; }

        [MaxLength(64)]
        [Column(TypeName = "varchar")]
        public string MicrosoftAccountProviderKeyWeb { get; set; }

        [MaxLength(64)]
        [Column(TypeName = "varchar")]
        public string MicrosoftAccountProviderKeyApi { get; set; }

        [MaxLength(200)]
        [Required]
        [Index(IsUnique = true)]
        public string Email { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public NotifyOptions MentionNotificationSettings { get; set; }

        [ForeignKey("Avatar")]
        public int? AvatarUserMediaId { get; set; }
        public virtual UserMedia Avatar { get; set; }

        [ForeignKey("Banner")]
        public int? BannerUserMediaId { get; set; }
        public virtual UserMedia Banner { get; set; }

        [ForeignKey("AvatarsAlbum")]
        public int? AvatarsMediaAlbumId { get; set; }
        public virtual MediaAlbum AvatarsAlbum { get; set; }

        [ForeignKey("BannersAlbum")]
        public int? BannersMediaAlbumId { get; set; }
        public virtual MediaAlbum BannersAlbum { get; set; }

        [ForeignKey("TimelineImagesAlbum")]
        public int? TimelineImagesMediaAlbumId { get; set; }
        public virtual MediaAlbum TimelineImagesAlbum { get; set; }

        public virtual IList<UserText> MentionedIn { get; set; }
    }
}
