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
        Task<IReadOnlyList<TaskDTO>> GetAllTasksAsync();
        Task<TaskDTO?> GetSingleTaskAsync(int id);
        Task<models.Task?> UpdateTaskStatusAsync(int id, string newStatus);

        Task<IReadOnlyList<TaskDTO>> GetAllTasksByStatusAsync(string status);
    }

    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<IReadOnlyList<TaskDTO>> GetAllTasksAsync()
        {
            return await _taskRepository.GetBaseTaskQuery().ToListAsync();
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

            ICollection<UserTask> RelatedUserTasks = await _taskRepository.GetRelatedUserTasks(id);

            return await _taskRepository.DeleteTaskAsync(RelatedUserTasks, task);
        }
    }
}
