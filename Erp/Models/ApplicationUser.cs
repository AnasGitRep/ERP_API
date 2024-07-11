using Erp.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.Models
{
    public class ApplicationUser: BaseEntity
    {

        public byte[]? Password { get; set; }
        public byte[]? PasswordKey { get; set; }

        public string? Email { get; set; }

        public string ? PhoneNumber { get; set; }
        
        public int UserImageId { get; set; }    
        public UserImage? UserImage { get; set; }

        public ICollection<UserPermissions>? UserPermissions { get; set; }
    }
}
