using System.Security;

namespace Erp.Models
{
    public class UserPermissions
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int PermissionId { get; set; }

        // Navigation properties
        public ApplicationUser? User { get; set; }
        public Permissions? Permissions { get; set; }
    }
}

