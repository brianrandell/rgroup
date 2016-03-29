using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rg.ServiceCore.DbModel
{
    /// <summary>
    /// Splits binary data for <see cref="UserMedia"/> into separate entity
    /// to make blob loading optional
    /// </summary>
    [Table("UserMedia")]
    public class UserMediaData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserMediaId { get; set; }

        public byte[] ImageData { get; set; }

        public virtual UserMedia UserMedia { get; set; }
    }
}
