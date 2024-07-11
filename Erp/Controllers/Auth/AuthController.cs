using Erp.Base;
using Erp.Dto;
using Erp.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;

namespace Erp.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _service;
        public AuthController(IAuthServices service)
        {
            _service = service;   
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody]RegisterDto user)
        {
          
            var result = await _service.Register(user);

            return Ok(result);

        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var result = await this._service.Login(model);
            return Ok(result);

        }

    }
}
