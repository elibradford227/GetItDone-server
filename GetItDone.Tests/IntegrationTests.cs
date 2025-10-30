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

public class IntegrationTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly TaskService _taskService;
    private readonly Mock<ITaskRepository> _mockTaskRepo;
    private readonly Mock<IUserTaskRepository> _mockUserTaskRepo;
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly GetItDoneDbContext _dbContext;


    public IntegrationTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockTaskRepo = new Mock<ITaskRepository>();
        _mockUserTaskRepo = new Mock<IUserTaskRepository>();
        _mockUserRepo = new Mock<IUserRepository>();

        var options = new DbContextOptionsBuilder<GetItDoneDbContext>()
            .UseInMemoryDatabase("IntegrationTests")
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new GetItDoneDbContext(options);
        _taskService = new TaskService(
            _mockUserTaskRepo.Object,
            _mockTaskRepo.Object,
            _dbContext,
            _mockUserRepo.Object);
    }

    [Fact]
    public async void  CreateTaskAsync_CreatesTaskAndCommitsTransaction()
    {
        // Arrange
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

        _mockUserTaskRepo
            .Setup(r => r.CheckUserTaskExists(It.IsAny<int>(), It.IsAny<string>()))
            .Returns(false);

        _mockUserTaskRepo
            .Setup(r => r.AddUserTaskAsync(It.IsAny<UserTask>()))
            .Callback<UserTask>(ut => _dbContext.UserTasks.Add(ut));

        var result = await _taskService.CreateTaskAsync(payload);

        Assert.NotNull(result);
        Assert.Equal("New Task", result.Title);
        Assert.Equal(1, _dbContext.Tasks.Count());
        Assert.Equal(2, _dbContext.UserTasks.Count());
    }
}
