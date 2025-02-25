using System.Threading.Tasks;
using AutoMapper;
using GetItDone.Data;
using GetItDone.models;
using GetItDone.models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GetItDone.repositories
{
    public interface ITaskRepository
    {
        Task<models.Task?> GetTaskById(int id);
        IQueryable<TaskDTO> GetBaseTaskQuery();
        Task<models.Task> UpdateTaskStatusAsync(models.Task task);
        Task<models.Task?> DeleteTaskAsync(models.Task TaskToDelete);
        Task<models.Task?> CreateTask(models.Task task);
    }

    public class TaskRepository : ITaskRepository
    {
        private readonly GetItDoneDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly IUserTaskRepository _userTaskRepository;

        public TaskRepository(GetItDoneDbContext dbContext, UserManager<User> userManager, IMapper mapper, IUserTaskRepository userTaskRepository)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _userTaskRepository = userTaskRepository;
        }

        public IQueryable<TaskDTO> GetBaseTaskQuery()
        {
            IQueryable<TaskDTO> taskQuery = _dbContext.Tasks
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
                });

              return taskQuery;
        }

        public async Task<models.Task> UpdateTaskStatusAsync(models.Task task)
        {
            _dbContext.Tasks.Update(task);
            await _dbContext.SaveChangesAsync();
            return task;
        }

        public async Task<models.Task?> CreateTask(models.Task task)
        {
            _dbContext.Tasks.Add(task);
            await _dbContext.SaveChangesAsync();
            return task;
        }

        public async Task<models.Task?> GetTaskById(int id)
        {
            return await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id);
        }
        public async Task<models.Task?> DeleteTaskAsync(models.Task taskToDelete)
        {
            _dbContext.Tasks.Remove(taskToDelete);

            await _dbContext.SaveChangesAsync();

            return taskToDelete;
        }
    }
}
