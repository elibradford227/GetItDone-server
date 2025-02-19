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
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace GetItDone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Identity.Application")]
    public class TaskController : ControllerBase
    {
        private readonly GetItDoneDbContext _dbContext;
        private readonly List<String> validStatuses = ["Complete", "In Progress", "Not Started"];
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
                    Status = t.Status,
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSingleTasks(int id)
        {
            TaskDTO Task = await _dbContext.Tasks
                .Include(t => t.Assignees)
                .Where(t => t.Id == id)
                .Select(t => new TaskDTO
                {
                    Id = t.Id,
                    Title = t.Title,
                    Status = t.Status,
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
                .FirstOrDefaultAsync();

            return Ok(Task);
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetTasksByStatus([FromQuery] string status)
        {
            IActionResult statusValidation = CheckValidStatus(status);

            if (statusValidation != null)
            {
                return statusValidation;
            }

            List<TaskDTO> Tasks = await _dbContext.Tasks
                .Include(t => t.Assignees)
                .Where(t => t.Status == status)
                .Select(t => new TaskDTO
                {
                    Id = t.Id,
                    Title = t.Title,
                    Status = t.Status,
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

        [HttpPut("status/change/{id}")]
        public async Task<IActionResult> UpdateTaskStatus(int id, [FromBody] TaskRequest request)
        {
            IActionResult statusValidation = CheckValidStatus(request.Status);

            if (statusValidation != null)
            {
                return statusValidation; 
            }

            models.Task TaskToUpdate = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id);

            if (TaskToUpdate == null)
            {
                return NotFound();
            }

            TaskToUpdate.Status = request.Status;

            await _dbContext.SaveChangesAsync();

            return Ok(TaskToUpdate);
        }

        private IActionResult CheckValidStatus(string status)
        {
            if (!validStatuses.Contains(status))
            {
                return BadRequest(new { message = "Status value must be 'Complete', 'In Progress', or 'Not Started'" });
            }

            return null;
        }

        public class TaskRequest
        {
            public string Status { get; set; }
        }
    }
}
