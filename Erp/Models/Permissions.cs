using Erp.Base;

namespace Erp.Models
{
    public class Permissions:BaseEntity
    {
        public ICollection<UserPermissions> UserPermissions { get; set; }

        public string? Description { get; set; } 
    }
}
