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
            IReadOnlyList<TaskDTO> tasks = await _taskService.GetAllTasksAsync();

            if (!tasks.Any())
            {
                return NotFound(new { message = "No tasks were found" });
            }

            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSingleTask(int id)
        {
            TaskDTO? task = await _taskService.GetSingleTaskAsync(id);

            if (task == null)
            {
                return NotFound(new { message = "Could not find task" });
            }

            return Ok(task);
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetTasksByStatus([FromQuery] string status)
        {
            BadRequestObjectResult? statusValidation = CheckValidStatus(status);

            if (statusValidation != null)
            {
                return statusValidation;
            }

            IReadOnlyList<TaskDTO> tasks = await _taskService.GetAllTasksByStatusAsync(status);

            return Ok(tasks);
        }

        [HttpPut("status/change/{id}")]
        public async Task<IActionResult> UpdateTaskStatus(int id, [FromBody] TaskRequest request)
        {
            BadRequestObjectResult? statusValidation = CheckValidStatus(request.Status);

            if (statusValidation != null)
            {
                return statusValidation;
            }

            models.Task? updatedTask = await _taskService.UpdateTaskStatusAsync(id, request.Status);

            if (updatedTask == null)
            {
                return NotFound();
            }

            return Ok(updatedTask);
        }

        [HttpPost("new")]
        public async Task<IActionResult> CreateTask([FromBody] TaskPayload taskPayload)
        {
            if (taskPayload == null)
            {
                return BadRequest(new { message = "Task Data must be sent" });
            }

            models.Task newTask = await _taskService.CreateTaskAsync(taskPayload);

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

        private BadRequestObjectResult? CheckValidStatus(string status)
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
