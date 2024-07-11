using Erp.Base;
using Erp.Dto;

namespace Erp.IServices
{
    public interface IAuthServices
    {
        Task<ResponseModel<RegisterDto>>Register(RegisterDto dto);

        Task<ResponseModel<LoginResponseDto>> Login(LoginDto dto);

        Task<bool> HasPermissionAsync(string permissionName, int userId);


    }
}
