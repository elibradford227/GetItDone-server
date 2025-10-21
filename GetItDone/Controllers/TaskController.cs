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
using GetItDone.Utils;

namespace GetItDone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Identity.Application")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly IMapper _mapper;
        private readonly List<String> validStatuses = ["Complete", "In Progress", "Not Started"];
        public TaskController(IMapper mapper, ITaskService taskService)
        {
            _mapper = mapper;
            _taskService = taskService;
        }

        //[HttpGet("all")]
        //public async Task<IActionResult> GetAllTasks()
        //{
        //    IReadOnlyList<TaskDTO> tasks = await _taskService.GetAllTasksAsync();

        //    if (!tasks.Any())
        //    {
        //        return NotFound(new { message = "No tasks were found" });
        //    }

        //    return Ok(tasks);
        //}

        [HttpGet]
        public async Task<IActionResult> GetAllPaginatedTasks([FromQuery] TaskQueryParams query)
        {
            IReadOnlyList<TaskDTO> tasks = await _taskService.GetAllTasksAsync(query.PageNumber, query.PageSize, query.Status);

            //int pageNumber = query.PageNumber;
            //int pageSize = query.PageSize;

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

            taskPayload.Title = TaskNormalizer.NormalizeTitle(taskPayload.Title);

            models.Task newTask = await _taskService.CreateTaskAsync(taskPayload);

            return Created($"/api/task/{newTask.Id}", newTask);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskPayload taskPayload)
        {
            // TODO: refactor to implement better error handling

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

            taskPayload.Title = TaskNormalizer.NormalizeTitle(taskPayload.Title);

            models.Task TaskToUpdate = await _taskService.UpdateTaskAsync(id, taskPayload, statusPassed);

            if (TaskToUpdate == null)
            {
                return NotFound();
            }

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

        public class TaskQueryParams
        {
            public int PageNumber { get; set; } = 1;
            public int PageSize { get; set; } = 10;
            public string? Status { get; set; } = null;
        }
    }
}
