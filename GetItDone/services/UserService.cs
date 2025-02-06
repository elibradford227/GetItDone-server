using GetItDone.Data;
using GetItDone.models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GetItDone.services
{
    //private readonly IUserRepository _userRepository;

    public interface IUserService
    {
        Task<User> FetchUser(string userId);
    }
    public class UserService: IUserService
    {
        //private readonly IUserRepository userRepository;

        private readonly UserManager<User> _userManager;

        public UserService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        public async Task<User> FetchUser(string userId)
        {
             User? user = await _userManager.FindByIdAsync(userId);
             return user;
        }
    }
}



