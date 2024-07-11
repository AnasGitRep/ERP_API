using Erp.Base;
using Erp.Data;
using Erp.Dto;
using Erp.IServices;
using Erp.Migrations;
using Erp.Models;
using Microsoft.EntityFrameworkCore;

namespace Erp.Services
{
    public class AppServicecs : IAppService
    {
        private readonly ApplicationDbContext _context;
        public AppServicecs(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ResponseModel<Permissions>> GetPermissions()
        {
            var responseModel = new ResponseModel<Permissions>();
            try
            {

                var result = await _context.Permissions
                                    .Select(u => new Permissions
                                    {
                                        Id = u.Id,
                                        Name = u.Name,
                                    })
                                    .ToListAsync();

                responseModel.Items = result;

                return responseModel;

            }
            catch (Exception ex) 
            {
                throw ex;
            }
        }

        public async Task<ResponseModel<UserDto>> GetUsers()
        {
            var responseModel = new ResponseModel<UserDto>();

            try
            {
                var result = await _context.Users
                    .Select(u => new UserDto
                    {
                        Id = u.Id,
                        Name = u.Name,
                    })
                    .ToListAsync();

                responseModel.Items = result;
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., logging)
                Console.WriteLine($"Error fetching users: {ex.Message}");
                // You might want to throw or handle this exception based on your application's error handling strategy
            }

            return responseModel;
        }
    }
}
