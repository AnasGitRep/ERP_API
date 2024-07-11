using Erp.Base;

namespace Erp.Dto
{
    public class LoginResponseDto:BaseEntity
    {
        public string ?Email { get; set; }
        
        public string? JwtToke {  get; set; }   

        public string ? ImageUrl { get; set; }  
        
    }
}
