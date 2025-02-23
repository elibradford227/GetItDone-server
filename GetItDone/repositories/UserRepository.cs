using GetItDone.Data;
using GetItDone.models;
using GetItDone.models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace GetItDone.repositories
{
    public interface IUserRepository
    {
        Task<UserDTO?> GetUserWithTasks(string userId);
    }
    public class UserRepository : IUserRepository
    {
        private readonly GetItDoneDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly ITaskRepository _taskRepository;
        private readonly IMapper _mapper;

        public UserRepository(GetItDoneDbContext dbContext, UserManager<User> userManager, ITaskRepository taskRepository, IMapper mapper)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _taskRepository = taskRepository;
            _mapper = mapper;
        }

        public async Task<UserDTO?> GetUserWithTasks(string userId)
        {
            User? user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return null;
            }

            List<UserTaskDTO> UsersTasks = await _taskRepository.GetUsersTasks(userId);

            var userDto = _mapper.Map<UserDTO>(user);

            userDto.Tasks = UsersTasks;

            return userDto;
        }
    }
}
