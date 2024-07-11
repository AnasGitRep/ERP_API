namespace Erp.Models
{
    public class UserImage
    {
        public int Id { get; set; } 
        public string? ImageUrl { get; set; }

        public int? UserId { get; set; }

        public ApplicationUser? ApplicationUser { get; set; }    
        

    }
}
