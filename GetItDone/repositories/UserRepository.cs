using GetItDone.Data;
using GetItDone.models;
using GetItDone.models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System.ComponentModel;
using static GetItDone.Controllers.TaskController;

namespace GetItDone.repositories
{
    public interface IUserRepository
    {
        Task<UserDTO?> GetUserWithTasks(string userId);
        bool CheckUserExists(string userId);
    }
    public class UserRepository : IUserRepository
    {
        private readonly GetItDoneDbContext _dbContext;
        private readonly IUserTaskRepository _userTaskRepository;
        private readonly IMapper _mapper;

        public UserRepository(GetItDoneDbContext dbContext, UserManager<User> userManager, IUserTaskRepository userTaskRepository, IMapper mapper)
        {
            _dbContext = dbContext;
            _userTaskRepository = userTaskRepository;
            _mapper = mapper;
        }

        public async Task<UserDTO?> GetUserWithTasks(string userId)
        {
            User? user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return null;
            }

            List<UserTaskDTO> UsersTasks = await _userTaskRepository.GetUsersTasks(userId);

            var userDto = _mapper.Map<UserDTO>(user);

            userDto.Tasks = UsersTasks;

            return userDto;
        }

        public bool CheckUserExists(string userId)
        {
            return _dbContext.Users.Any(u => u.Id == userId);
        }
    }
}
