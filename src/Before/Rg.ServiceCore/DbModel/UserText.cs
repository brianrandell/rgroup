using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rg.ServiceCore.DbModel
{
    public class UserText
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserTextId { get; set; }

        [Required]
        public string Content { get; set; }

        [Index]
        public DateTime CreatedUtc { get; set; }

        public virtual IList<UserInfo> MentionsUser { get; set; }

        public virtual IList<Like> Likes { get; set; }
    }
}
