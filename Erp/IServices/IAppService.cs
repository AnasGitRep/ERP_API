using Erp.Base;
using Erp.Dto;
using Erp.Migrations;
using Erp.Models;

namespace Erp.IServices
{
    public interface IAppService
    {
        Task<ResponseModel<Permissions>> GetPermissions();

        Task<ResponseModel<UserDto>> GetUsers();
    }
}
