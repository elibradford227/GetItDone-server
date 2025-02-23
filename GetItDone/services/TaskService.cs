using GetItDone.models;
using GetItDone.models.DTOs;
using GetItDone.repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GetItDone.services
{
    public interface ITaskService
    {
        Task<models.Task?> RemoveTaskAsync(int id);
        Task<List<TaskDTO>> GetAllTasksAsync();
        Task<TaskDTO?> GetSingleTaskAsync(int id);

        Task<IReadOnlyList<TaskDTO>> GetAllTasksByStatusAsync(string status);
    }

    public class TaskService : ITaskService
    {
        private readonly UserManager<User> _userManager;
        private readonly ITaskRepository _taskRepository;

        public TaskService(UserManager<User> userManager, ITaskRepository taskRepository)
        {
            _userManager = userManager;
            _taskRepository = taskRepository;
        }

        public async Task<List<TaskDTO>> GetAllTasksAsync()
        {
            return await _taskRepository.GetBaseTaskQuery().ToListAsync();
        }

        public async Task<IReadOnlyList<TaskDTO>> GetAllTasksByStatusAsync(string status)
        {
            return await _taskRepository.GetBaseTaskQuery().Where(t => t.Status == status).ToListAsync();
        }

        public async Task<TaskDTO?> GetSingleTaskAsync(int id)
        {
            return await _taskRepository.GetBaseTaskQuery().Where(t => t.Id == id).SingleOrDefaultAsync();
        }

        public async Task<models.Task?> RemoveTaskAsync(int id)
        {
            models.Task? task = await _taskRepository.GetTaskById(id);

            if (task == null)
            {
                return null;
            }

            ICollection<UserTask> RelatedUserTasks = await _taskRepository.GetRelatedUserTasks(id);

            return await _taskRepository.DeleteTaskAsync(RelatedUserTasks, task);
        }
    }
}
