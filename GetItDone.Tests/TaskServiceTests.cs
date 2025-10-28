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

public class TaskServiceTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly TaskService _taskService;
    private readonly Mock<ITaskRepository> _mockTaskRepo;
    private readonly Mock<IUserTaskRepository> _mockUserTaskRepo;
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly GetItDoneDbContext _dbContext;


    public TaskServiceTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockTaskRepo = new Mock<ITaskRepository>();
        _mockUserTaskRepo = new Mock<IUserTaskRepository>();
        _mockUserRepo = new Mock<IUserRepository>();

        var options = new DbContextOptionsBuilder<GetItDoneDbContext>()
            .UseInMemoryDatabase("TaskServiceTestsDB")
            .Options;

        _dbContext = new GetItDoneDbContext(options);
        _taskService = new TaskService( 
            _mockUserTaskRepo.Object,
            _mockTaskRepo.Object,
            _dbContext,
            _mockUserRepo.Object);
    }

    [Fact]
    public async void UpdateTaskStatusAsyncReturnsUpdatedTask()
    {
        int id = 1;
        string newStatus = "Pending";
        var existingTask = new models.Task { Id = id, Status = newStatus };

        _mockTaskRepo.Setup(r => r.GetTaskById(id))
            .ReturnsAsync(existingTask);
        _mockTaskRepo.Setup(r => r.UpdateTaskStatusAsync(It.IsAny<models.Task>()))
            .ReturnsAsync((models.Task t) => t);

        var result = await _taskService.UpdateTaskStatusAsync(id, newStatus);

        Assert.NotNull(result);
        Assert.Equal(newStatus, result.Status);
        _mockTaskRepo.Verify(r => r.GetTaskById(id), Times.Once);
        _mockTaskRepo.Verify(r => r.UpdateTaskStatusAsync(It.Is<models.Task>(t => t.Status == newStatus)), Times.Once);
    }
}
