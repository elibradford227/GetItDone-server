using GetItDone.models;
using GetItDone.models.DTOs;
using GetItDone.repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GetItDone.services
{
    public interface IUserTaskService
    {

    }

    public class UserTaskService : IUserTaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IUserTaskRepository _userTaskRepository;

        public UserTaskService(IUserTaskRepository userTaskRepository, ITaskRepository taskRepository)
        {
            _userTaskRepository = userTaskRepository;
            _taskRepository = taskRepository;
        }


    }
}
