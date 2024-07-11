using Erp.Dto;
using Erp.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppController : ControllerBase
    {
        private readonly IAppService _service;
        public AppController(IAppService service)
        {
            _service = service;
        }

        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {

            var result = await _service.GetUsers();

            return Ok(result);

        }


        [HttpGet("GetPermissions")]
        public async Task<IActionResult> GetPermissions()
        {
            var result = await this._service.GetPermissions();
            return Ok(result);

        }
    }
}
