using Erp.Base;
using Erp.Dto;
using Erp.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Erp.Controllers.Auth
{
    [Authorize(Roles ="Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminServices _adminServices;
        private readonly IAuthServices _authServices;
        public AdminController(IAdminServices adminservice,IAuthServices Authservice )
        {
            _adminServices = adminservice;
            _authServices = Authservice;    
            
        }


        [HttpPost("AddEmployee")]
        public async Task<IActionResult> Register([FromBody] RegisterDto user)
        {
            ResponseModel<RegisterDto> response=new ResponseModel<RegisterDto>();   

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var adminId = GetCurrentAdminId();
                // Check if current user has permission to add employees
                if (!await _authServices.HasPermissionAsync("CanAdd",adminId))
                {
                    response.IsOk = false;
                    response.Message = "You dont have permission add Employee";
                    return Ok(response);
                }
                var result = true;

                if (result == null)
                {
                    return BadRequest("Failed to add employee");
                }

                response.IsOk = true;
                response.Message = "Employee added Sucsessfully";
                return Ok(response);

            }catch(Exception ex) {
                throw ex;
            }

        }
        [HttpPost("{userId}/permission/update")]
        public async Task<IActionResult> UpdateUserPermissions(int userId, [FromBody] List<string> permissions)
        {
            var result = await _adminServices.UpdateUserPermissionsAsync(userId, permissions);
            if (result.IsOk==false)
            {
                return NotFound("User not found");
            }

            return Ok(result);
        }

        [HttpPost("{userId}/permission/add")]
        public async Task<IActionResult> AddUserPermissions(int userId, [FromBody] List<string> permissions)
        {
            var result = await _adminServices.AddUserPermissionsAsync(userId, permissions);
            if (result.IsOk==false)
            {
                return NotFound("User not found");
            }

            return Ok(result);
        }



        private int GetCurrentAdminId()
        {
            var claim = User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier);
            if (claim != null && int.TryParse(claim.Value, out int adminId))
            {
                return adminId;
            }
            throw new ApplicationException("Admin ID claim not found or invalid.");
        }
    }
}
