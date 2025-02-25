using System.Threading.Tasks;
using AutoMapper;
using GetItDone.Data;
using GetItDone.models;
using GetItDone.models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GetItDone.repositories
{
    public interface IUserTaskRepository
    {
        Task<List<UserTaskDTO>> GetUsersTasks(string userId);
        bool CheckUserTaskExists(int taskId, string userId);
        Task<ICollection<UserTask>> GetRelatedUserTasks(int taskId);
        System.Threading.Tasks.Task RemoveRelatedUserTasks(ICollection<UserTask> RelatedUserTasks);
        System.Threading.Tasks.Task AddUserTaskToContext(UserTask userTask);
        }
    public class UserTaskRepository : IUserTaskRepository
    {
        private readonly GetItDoneDbContext _dbContext;
        private readonly UserManager<User> _userManager;

        public UserTaskRepository(GetItDoneDbContext dbContext, UserManager<User> userManager, IMapper mapper)
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

        public bool CheckUserTaskExists(int taskId, string userId)
        {
            return _dbContext.UserTasks.Any(ut => ut.TaskId == taskId && ut.UserId == userId);
        }

        public async Task<ICollection<UserTask>> GetRelatedUserTasks(int taskId)
        {
            return await _dbContext.UserTasks
               .Where(t => t.TaskId == taskId)
               .ToListAsync();
        }

        public async System.Threading.Tasks.Task RemoveRelatedUserTasks(ICollection<UserTask> RelatedUserTasks)
        {
            _dbContext.UserTasks.RemoveRange(RelatedUserTasks);
            await _dbContext.SaveChangesAsync();
        }


        public async System.Threading.Tasks.Task AddUserTaskToContext(UserTask userTask)
        {
            _dbContext.UserTasks.Add(userTask);
        }
    }
}
