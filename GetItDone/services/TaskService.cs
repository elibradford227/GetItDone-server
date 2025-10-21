using GetItDone.Data;
using GetItDone.models;
using GetItDone.models.DTOs;
using GetItDone.repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using static GetItDone.Controllers.TaskController;

namespace GetItDone.services
{
    public interface ITaskService
    {
        Task<models.Task?> RemoveTaskAsync(int id);
        Task<IReadOnlyList<TaskDTO>> GetAllTasksAsync(int pageNumber, int pageSize, string? status);
        Task<TaskDTO?> GetSingleTaskAsync(int id);
        Task<models.Task?> UpdateTaskStatusAsync(int id, string newStatus);
        Task<models.Task> CreateTaskAsync(TaskPayload taskPayload);
        Task<IReadOnlyList<TaskDTO>> GetAllTasksByStatusAsync(string status);
        Task<models.Task> UpdateTaskAsync(int id, TaskPayload taskPayload, bool statusPassed);
    }

    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IUserTaskRepository _userTaskRepository;
        private readonly IUserRepository _userRepository;
        private readonly GetItDoneDbContext _dbContext;

        public TaskService(IUserTaskRepository userTaskRepository, ITaskRepository taskRepository, GetItDoneDbContext dbContext, IUserRepository userRepository)
        {
            _userTaskRepository = userTaskRepository;
            _taskRepository = taskRepository;
            _userRepository = userRepository;
            _dbContext = dbContext;
        }

        //public async Task<IReadOnlyList<TaskDTO>> GetAllTasksAsync()
        //{
        //    return await _taskRepository.GetBaseTaskQuery().ToListAsync();
        //}

        public async Task<IReadOnlyList<TaskDTO>> GetAllTasksAsync(int pageNumber, int pageSize, string? status = null)
        {
            return await _taskRepository.GetPaginatedTasks(pageNumber, pageSize, status).ToListAsync();
        }

        public async Task<IReadOnlyList<TaskDTO>> GetAllTasksByStatusAsync(string status)
        {
            return await _taskRepository.GetBaseTaskQuery().Where(t => t.Status == status).ToListAsync();
        }

        public Task<TaskDTO?> GetSingleTaskAsync(int id)
        {
            return _taskRepository.GetBaseTaskQuery().Where(t => t.Id == id).SingleOrDefaultAsync();
        }

        public async Task<models.Task?> UpdateTaskStatusAsync(int id, string newStatus)
        {
            models.Task? taskToUpdate = await _taskRepository.GetTaskById(id);

            if (taskToUpdate == null)
            {
                return null;
            }

            taskToUpdate.Status = newStatus;

            return await _taskRepository.UpdateTaskStatusAsync(taskToUpdate);
        }

        public async Task<models.Task?> RemoveTaskAsync(int id)
        {
            models.Task? task = await _taskRepository.GetTaskById(id);

            if (task == null)
            {
                return null;
            }

            return await _taskRepository.DeleteTaskAsync(task);
        }

        public async Task<models.Task> CreateTaskAsync(TaskPayload taskPayload)
        {
            using IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                models.Task newTask = new models.Task
                {
                    Title = taskPayload.Title,
                    Description = taskPayload.Description,
                    Status = taskPayload.Status,
                    Ownerid = taskPayload.Ownerid,
                    DueDate = taskPayload.DueDate,
                };

                _dbContext.Tasks.Add(newTask);

                if (taskPayload.Assignees?.Any() == true)
                {
                    await CreateAssignees(taskPayload, newTask);
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return newTask;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<models.Task> UpdateTaskAsync(int id, TaskPayload taskPayload, bool statusPassed)
        {
            models.Task? TaskToUpdate = await _taskRepository.GetTaskById(id);

            if (TaskToUpdate == null)
            {
                return null;
            }

            if (taskPayload.Ownerid != null)
            {
                bool userExists = _userRepository.CheckUserExists(taskPayload.Ownerid);

                if (!userExists)
                {
                    return null;
                }
            }

            TaskToUpdate.Title = taskPayload.Title;
            TaskToUpdate.Description = taskPayload.Description;
            TaskToUpdate.Ownerid = taskPayload.Ownerid != null ? taskPayload.Ownerid : TaskToUpdate.Ownerid;
            TaskToUpdate.Status = statusPassed ? taskPayload.Status : TaskToUpdate.Status;
            TaskToUpdate.DueDate = taskPayload.DueDate;

            if (taskPayload.Assignees?.Any() == true)
            {
                await CreateAssignees(taskPayload, TaskToUpdate);
            }

            await _dbContext.SaveChangesAsync();
            return TaskToUpdate;
        }


        private async System.Threading.Tasks.Task CreateAssignees(TaskPayload taskPayload, models.Task task)
        {

            foreach (AssigneePayload assignee in taskPayload.Assignees)
            {
                bool userTaskExists = _userTaskRepository.CheckUserTaskExists(task.Id, assignee.UserId);

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
    }
}
