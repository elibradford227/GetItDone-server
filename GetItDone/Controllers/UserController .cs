using GetItDone.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GetItDone.models;
using System.Security.Claims;
using GetItDone.services;

namespace GetItDone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Identity.Application")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetUserById(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return BadRequest(new { message = "UserId is required." });
            }

            return await GetUserByIdInternal(Id);
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetAuthenticatedUser()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User is not authenticated." });
            }

            return await GetUserByIdInternal(userId); 
        }

        private async Task<IActionResult> GetUserByIdInternal(string userId)
        {
            User? user = await _userService.FetchUser(userId);
            if (user == null)
            {
                    return NotFound(new { message = "User not found." });
            }
            return Ok(user);
        }
    }
}
