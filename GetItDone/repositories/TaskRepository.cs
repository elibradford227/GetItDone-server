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
        Task<List<UserTaskDTO>> GetUsersTasks(string userId);
        Task<models.Task?> GetTaskById(int id);
        Task<ICollection<UserTask>> GetRelatedUserTasks(int taskId);
        Task<models.Task?> DeleteTaskAsync(ICollection<UserTask> RelatedUserTasks, models.Task TaskToDelete);
    }

    public class TaskRepository : ITaskRepository
    {
        private readonly GetItDoneDbContext _dbContext;
        private readonly UserManager<User> _userManager;

        public TaskRepository(GetItDoneDbContext dbContext, UserManager<User> userManager, IMapper mapper)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<List<UserTaskDTO>> GetUsersTasks(string userId)
        {
            List<UserTaskDTO> UsersTasks = await _dbContext.UserTasks
                .Include(u => u.Task)
                .Where(u => u.UserId == userId)
                .Select(u => new UserTaskDTO
                {
                    Id = u.Id,
                    TaskId = u.TaskId,
                    Task = new TaskDTO
                    {
                        Id = u.Task.Id,
                        Title = u.Task.Title,
                        Description = u.Task.Description,
                        Status = u.Task.Status,
                    }
                })
                .ToListAsync();

            return UsersTasks;
        }

        public async Task<models.Task?> GetTaskById(int id)
        {
            return await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<ICollection<UserTask>> GetRelatedUserTasks(int taskId)
        {
            return await _dbContext.UserTasks
               .Where(t => t.TaskId == taskId)
               .ToListAsync();
        }

        public async Task<models.Task?> DeleteTaskAsync(ICollection<UserTask> RelatedUserTasks, models.Task taskToDelete)
        {
            if (RelatedUserTasks.Count > 0)
            {
                _dbContext.UserTasks.RemoveRange(RelatedUserTasks);
            }

            _dbContext.Tasks.Remove(taskToDelete);

            await _dbContext.SaveChangesAsync();

            return taskToDelete;
        }
    }
}
