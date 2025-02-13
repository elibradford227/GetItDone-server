using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GetItDone.models;
using System.Security.Claims;
using GetItDone.services;
using GetItDone.models.DTOs;
using AutoMapper;

namespace GetItDone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Identity.Application")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;   
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

        [HttpGet("tasks/{id}")]
        public async Task<IActionResult> GetUserWithTasks(string id)
        {
            UserDTO user = await _userService.FetchUserWithTasks(id);

            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var userDto = _mapper.Map<UserDTO>(user);
            return Ok(userDto);
        }

        private async Task<IActionResult> GetUserByIdInternal(string userId)
        {
            User? user = await _userService.FetchUser(userId);
            if (user == null)
            {
                    return NotFound(new { message = "User not found." });
            }
            var userDto = _mapper.Map<UserDTO>(user);
            return Ok(userDto);
        }
    }
}
