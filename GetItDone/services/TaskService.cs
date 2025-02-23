using GetItDone.models;
using GetItDone.repositories;
using Microsoft.AspNetCore.Identity;

namespace GetItDone.services
{
    public interface ITaskService
    {
        Task<models.Task?> GetTaskById(int id);
        Task<models.Task?> RemoveTaskAsync(int id);
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

        public async Task<models.Task?> GetTaskById(int id)
        {
            return await _taskRepository.GetTaskById(id);
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
