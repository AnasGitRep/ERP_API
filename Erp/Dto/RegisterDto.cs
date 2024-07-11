using Erp.Base;
using Erp.Models;
using System.ComponentModel.DataAnnotations;

namespace Erp.Dto
{
    public class RegisterDto:BaseEntity
    {

        public string? Password { get; set; }

        [EmailAddress(ErrorMessage = "Provide a valid email address")]
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ImageUrl { get; set; }
    }
}
