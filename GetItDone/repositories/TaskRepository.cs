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

        //public async Task<User> GetUsersTasks(string userId)
        //{
        //    User? user = await _userManager.Users
        //        .Include(u => u.Tasks)
        //        .ThenInclude(ut => ut.Task)
        //        .FirstOrDefaultAsync(u => u.Id == userId);
        //    return user;
        //}
    }
}
