namespace GetItDone.Tests;

using Xunit;
using Moq;
using GetItDone.Controllers;
using GetItDone.models;
using GetItDone.models.DTOs;
using GetItDone.repositories;
using GetItDone.services;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using GetItDone.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

using static GetItDone.Controllers.TaskController;
using Microsoft.AspNetCore.Identity;

public class IntegrationTests
{
    private readonly GetItDoneDbContext _dbContext;
    private readonly TaskRepository _taskRepo;
    private readonly UserTaskRepository _userTaskRepo;
    private readonly UserRepository _userRepo;
    private readonly TaskService _taskService;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;


    public IntegrationTests()
    {
        var options = new DbContextOptionsBuilder<GetItDoneDbContext>()
            .UseInMemoryDatabase("IntegrationTests")
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new GetItDoneDbContext(options);
        _userTaskRepo = new UserTaskRepository(_dbContext, _userManager, _mapper);
        _taskRepo = new TaskRepository(_dbContext, _userManager, _mapper, _userTaskRepo);
        _userRepo = new UserRepository(_dbContext, _userManager, _userTaskRepo, _mapper);

        _taskService = new TaskService(_userTaskRepo, _taskRepo, _dbContext, _userRepo);
    }

    [Fact]
    public async void  CreateTaskAsync_CreatesTaskAndCommitsTransaction()
    {
        var payload = new TaskPayload
        {
            Title = "New Task",
            Description = "Integration test",
            Status = "Open",
            Ownerid = "1",
            DueDate = DateTime.UtcNow.AddDays(5),
            Assignees = new List<AssigneePayload>
            {
                new AssigneePayload { UserId = "1" },
                new AssigneePayload { UserId = "2" }
            }
        };

        var result = await _taskService.CreateTaskAsync(payload);

        var savedTask = await _dbContext.Tasks
            .Include(t => t.Assignees)
            .FirstOrDefaultAsync(t => t.Id == result.Id);

        Assert.NotNull(result);
        Assert.Equal("New Task", result.Title);
        Assert.Equal(1, _dbContext.Tasks.Count());
        Assert.Equal(2, _dbContext.UserTasks.Count());
        Assert.All(savedTask.Assignees, ut => Assert.Equal(savedTask.Id, ut.TaskId));
    }
}
