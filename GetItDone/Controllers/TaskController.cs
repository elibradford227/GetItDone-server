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
using System.ComponentModel.DataAnnotations;
using AutoMapper;

namespace GetItDone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Identity.Application")]
    public class TaskController : ControllerBase
    {
        private readonly GetItDoneDbContext _dbContext;
        private readonly ITaskService _taskService;
        private readonly IMapper _mapper;
        private readonly List<String> validStatuses = ["Complete", "In Progress", "Not Started"];
        public TaskController(GetItDoneDbContext dbContext, IMapper mapper, ITaskService taskService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _taskService = taskService; 
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
                    Ownerid = t.Ownerid,
                    Assignees = t.Assignees.Select(a => new UserTaskDTO
                    {
                        Id = a.Id,
                        UserId = a.UserId,
                        TaskId = a.TaskId,
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

        [HttpPost("new")]
        public async Task<IActionResult> CreateTask([FromBody] TaskPayload taskPayload)
        {
            if (taskPayload == null)
            {
                return BadRequest(new { message = "Task Data must be sent" });
            }


            models.Task newTask = new models.Task
            {
                Title = taskPayload.Title,
                Description = taskPayload.Description,
                Status  = taskPayload.Status,
                Ownerid = taskPayload.Ownerid,  
                DueDate = taskPayload.DueDate,
            };

            _dbContext.Tasks.Add(newTask);

            // Confirm if assignees were passed and create user task entities if they were 
            if (taskPayload.Assignees?.Any() == true)
            {
                CreateAssignees(taskPayload, newTask);
            }

            await _dbContext.SaveChangesAsync();

            return Created($"/api/task/{newTask.Id}", null);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskPayload taskPayload)
        {
            if (taskPayload == null)
            {
                return BadRequest(new { message = "Task Data must be sent" });
            }

            Boolean statusPassed = false;

            if (!string.IsNullOrEmpty(taskPayload.Status))
            {
                statusPassed = true;

                IActionResult statusValidation = CheckValidStatus(taskPayload.Status);

                if (statusValidation != null)
                {
                    return statusValidation;
                }
            }

            models.Task TaskToUpdate = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id);

            if (TaskToUpdate == null)
            {
                return NotFound();
            }

            if (taskPayload.Ownerid != null)
            {
                bool userExists = _dbContext.Users.Any(u => u.Id == taskPayload.Ownerid);

                if (!userExists)
                {
                    return BadRequest(new { message = "Owner id must be a valid user" });
                }
            }

            TaskToUpdate.Title = taskPayload.Title;
            TaskToUpdate.Description = taskPayload.Description;
            TaskToUpdate.Ownerid = taskPayload.Ownerid != null ? taskPayload.Ownerid : TaskToUpdate.Ownerid;
            TaskToUpdate.Status = statusPassed ? taskPayload.Status : TaskToUpdate.Status;
            TaskToUpdate.DueDate = taskPayload.DueDate;

            if (taskPayload.Assignees?.Any() == true)
            {
                CreateAssignees(taskPayload, TaskToUpdate);
            }

            await _dbContext.SaveChangesAsync();

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            models.Task? taskDeleted = await _taskService.RemoveTaskAsync(id);

            if (taskDeleted == null)
            {
                return NotFound(new { message = $"Task was not found" });
            }

            return Ok(new { message = $"Task {taskDeleted.Title} was deleted" });
        }

        private void CreateAssignees(TaskPayload taskPayload, models.Task task)
        {

            foreach (AssigneePayload assignee in taskPayload.Assignees)
            {
                bool userTaskExists = _dbContext.UserTasks.Any(ut => ut.TaskId == task.Id && ut.UserId == assignee.UserId);

                if (!userTaskExists)
                {
                    UserTask newUserTask = new UserTask
                    {
                        UserId = assignee.UserId,
                        Task = task
                    };

                    _dbContext.UserTasks.Add(newUserTask);
                }
            }
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

        public class AssigneePayload
        {
            public string UserId { get; set; }
        }
        public class TaskPayload
        {
            public string Title { get; set; }
            public string? Description { get; set; }
            public string? Status { get; set; }
            public string? Ownerid { get; set; }
            public DateTime CreatedDate { get; set; }
            public DateTime? DueDate { get; set; }
            public List<AssigneePayload> Assignees { get; set; }
        }
    }
}
