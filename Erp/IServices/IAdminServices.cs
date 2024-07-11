using Erp.Base;
using Erp.Models;

namespace Erp.IServices
{
    public interface IAdminServices
    {
        Task<ResponseModel<Permissions>> AddUserPermissionsAsync(int userId, List<string> permissions);
        Task<ResponseModel<Permissions>> UpdateUserPermissionsAsync(int userId, List<string> permissions);
    }
}

