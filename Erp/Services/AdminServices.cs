using Erp.Base;
using Erp.Data;
using Erp.IServices;
using Erp.Models;
using Microsoft.EntityFrameworkCore;

namespace Erp.Services
{
    public class AdminServices : IAdminServices
    {
        private readonly ApplicationDbContext _dbcontext;
        public AdminServices(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;   
        }





        public async Task<ResponseModel<Permissions>> AddUserPermissionsAsync(int userId, List<string> permissions)
        {
            var responseModel = new ResponseModel<Permissions>();

            try
            {
                var user = await _dbcontext.Users.FirstOrDefaultAsync(x => x.Id == userId);
                if (user == null)
                {
                    responseModel.IsOk = false;
                    responseModel.Message = "User not found";
                    return responseModel;
                }

                var addedPermissions = new List<Permissions>();

                foreach (var permission in permissions)
                {
                    var permissionEntity = await _dbcontext.Permissions.FirstOrDefaultAsync(p => p.Name == permission);
                    if (permissionEntity != null)
                    {
                        await _dbcontext.UserPermissions.AddAsync(new UserPermissions
                        {
                            UserId = userId,
                            PermissionId = permissionEntity.Id
                        });
                        addedPermissions.Add(permissionEntity);
                    }
                }

                await _dbcontext.SaveChangesAsync();
                responseModel.IsOk = true;
                responseModel.Message = "Permissions added successfully";
                
            }
            catch (Exception ex)
            {
                responseModel.IsOk = false;
                responseModel.Message = $"Error adding permissions: {ex.Message}";
            }

            return responseModel;
        }



        public async Task<ResponseModel<Permissions>> UpdateUserPermissionsAsync(int userId, List<string> permissions)
        {
            var responseModel = new ResponseModel<Permissions>();

            try
            {
                var user = await _dbcontext.Users.FirstOrDefaultAsync(x => x.Id == userId);
                if (user == null)
                {
                    responseModel.IsOk = false;
                    responseModel.Message = "User not found";
                    return responseModel;
                }

                // Assuming you want to clear existing permissions first
                var existingUserPermissions = _dbcontext.UserPermissions.Where(up => up.UserId == userId);
                _dbcontext.UserPermissions.RemoveRange(existingUserPermissions);

                var updatedPermissions = new List<Permissions>();

                foreach (var permission in permissions)
                {
                    var permissionEntity = await _dbcontext.Permissions.FirstOrDefaultAsync(p => p.Name == permission);
                    if (permissionEntity != null)
                    {
                        await _dbcontext.UserPermissions.AddAsync(new UserPermissions
                        {
                            UserId = userId,
                            PermissionId = permissionEntity.Id
                        });
                        updatedPermissions.Add(permissionEntity);
                    }
                }

                await _dbcontext.SaveChangesAsync();
                responseModel.IsOk = true;
                responseModel.Message = "Permissions updated successfully";
                
            }
            catch (Exception ex)
            {
                responseModel.IsOk = false;
                responseModel.Message = $"Error updating permissions: {ex.Message}";
            }

            return responseModel;
        }

    }
}
