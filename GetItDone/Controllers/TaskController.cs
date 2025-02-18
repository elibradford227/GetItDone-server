using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GetItDone.models;
using System.Security.Claims;
using GetItDone.services;
using GetItDone.models.DTOs;
using AutoMapper;
using GetItDone.Data;
using Microsoft.EntityFrameworkCore;

namespace GetItDone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Identity.Application")]
    public class TaskController : ControllerBase
    {
        private readonly GetItDoneDbContext _dbContext;
        public TaskController(GetItDoneDbContext dbContext) 
        {
            _dbContext = dbContext;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllTasks()
        {
            List<TaskDTO> Tasks = await _dbContext.Tasks
                .Include(t => t.Assignees)
                .Select(t => new TaskDTO
                {
                    Id = t.Id,
                    Title = t.Title,
                    Assignees = t.Assignees.Select(a => new UserTaskDTO
                    {
                        Id = a.Id,
                        UserId = a.UserId,
                        User = a.User != null ? new UserDTO
                        {
                           Id = a.User.Id,
                           UserName = a.User.UserName
                        } : null
                    }).ToList()
                })
                .ToListAsync();
            return Ok(Tasks);
        }

        [HttpGet("complete")]
        public async Task<IActionResult> GetAllCompleteTasks()
        {
            List<TaskDTO> Tasks = await _dbContext.Tasks
                .Include(t => t.Assignees)
                .Select(t => new TaskDTO
                {
                    Id = t.Id,
                    Title = t.Title,
                    Assignees = t.Assignees.Select(a => new UserTaskDTO
                    {
                        Id = a.Id,
                        UserId = a.UserId,
                        User = a.User != null ? new UserDTO
                        {
                            Id = a.User.Id,
                            UserName = a.User.UserName
                        } : null
                    }).ToList()
                })
                .ToListAsync();
            return Ok(Tasks);
        }
    }
}
