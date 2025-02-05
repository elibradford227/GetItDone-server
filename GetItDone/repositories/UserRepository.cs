using GetItDone.Data;
using GetItDone.models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GetItDone.repositories
{
    public class UserRepository
    {
        private readonly GetItDoneDbContext _dbContext;

        public UserRepository(GetItDoneDbContext dbContext)
        {
            _dbContext = dbContext;
        }

 
    }
}
