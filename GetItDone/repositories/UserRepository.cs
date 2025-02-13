using GetItDone.Data;
using GetItDone.models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GetItDone.repositories
{
    public class UserRepository
    {
        private readonly GetItDoneDbContext _dbContext;
        private readonly UserManager<User> _userManager;

        public UserRepository(GetItDoneDbContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        //public async Task<User> GetUsersTasks(userId)
        //{
        //    User? user = await _userManager.Users
        //        .Include(u => u.Tasks)
        //        .ThenInclude(ut => ut.Task)
        //        .FirstOrDefaultAsync(u => u.Id == userId);
        //    return user;


    
 
    }
}
