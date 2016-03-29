using System.ComponentModel.DataAnnotations;

namespace Rg.ServiceCore.DbModel
{
    public class Invitation
    {
        [Key]
        [MaxLength(200)]
        public string Email { get; set; }
    }
}
