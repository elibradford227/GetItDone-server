using GetItDone.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GetItDone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Identity.Application")]
    public class AuthController : ControllerBase
    {

        // Injected for now in case of future need
        private GetItDoneDbContext _dbContext;

        public AuthController(GetItDoneDbContext context)
        {
            _dbContext = context;
        }

        [HttpGet]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
             await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
             return Ok();
        }
    }
}
