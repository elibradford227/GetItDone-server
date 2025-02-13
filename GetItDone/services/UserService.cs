using GetItDone.Data;
using GetItDone.models;
using GetItDone.repositories;
using Microsoft.AspNetCore.Identity;
using GetItDone.models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace GetItDone.services
{
    public interface IUserService
    {
        Task<User> FetchUser(string userId);
        Task<UserDTO> FetchUserWithTasks(string userId);
    }

    public class UserService: IUserService
    {

        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;

        public UserService(UserManager<User> userManager, IUserRepository userRepository)
        {
            _userManager = userManager;
            _userRepository = userRepository;
        }
        public async Task<User> FetchUser(string userId)
        {
             User? user = await _userManager.FindByIdAsync(userId);
             return user;
        }

        public async Task<UserDTO> FetchUserWithTasks(string userId)
        {
            UserDTO user = await _userRepository.GetUserWithTasks(userId);
            return user;
        }
    }
}



