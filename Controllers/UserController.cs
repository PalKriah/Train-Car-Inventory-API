using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Train_Car_Inventory_App.Models.Payloads;
using Train_Car_Inventory_App.Services;

namespace Train_Car_Inventory_App.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest data)
        {
            var result = await _userService.LoginAsync(data);
            return Ok(result);
        }
        
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequest data)
        {
            var result = await _userService.RegisterAsync(data);
            return Ok("Successful registration");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            return Ok(_userService.Logout());
        }
    }
}