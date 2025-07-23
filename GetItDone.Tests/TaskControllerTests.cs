namespace GetItDone.Tests;

using Xunit;
using Moq;
using GetItDone.Controllers;
using GetItDone.models;
using GetItDone.models.DTOs;
using GetItDone.repositories;
using GetItDone.services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

public class TaskControllerTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ITaskService> _mockService;
    private readonly TaskController _controller;

    public TaskControllerTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockService = new Mock<ITaskService>();
        _controller = new TaskController(_mockMapper.Object, _mockService.Object);
    }


    [Fact]
    public async void GetAllTasksReturnsOk()
    {

        List<TaskDTO> expectedTasks = new List<TaskDTO>
        { new TaskDTO { Id = 1, Title = "Test" }, 
          new TaskDTO { Id = 2, Title = "Test 2" },
        };

        _mockService.Setup(s => s.GetAllTasksAsync()).ReturnsAsync(expectedTasks);

        var result = await _controller.GetAllTasks();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var tasks = Assert.IsAssignableFrom<IEnumerable<TaskDTO>>(okResult.Value);
        Assert.Equal(2, tasks.Count());
    }

    [Fact]
    public async void GetSingleTaskReturnsOk()
    {

        TaskDTO expectedTask = new TaskDTO { Id = 1, Title = "Test" };

        _mockService.Setup(s => s.GetSingleTaskAsync(1)).ReturnsAsync(expectedTask);

        var result = await _controller.GetSingleTask(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<TaskDTO>(okResult.Value);
        Assert.Equal(expectedTask.Id, value.Id);
    }

    [Fact] 
    public async void GetSingleTask_ReturnsNotFound_WhenTaskDoesNotExist()
    {
        _mockService.Setup(s => s.GetSingleTaskAsync(1)).ReturnsAsync((TaskDTO?)null);

        var result = await _controller.GetSingleTask(1);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(404, notFound.StatusCode);
    }

    [Fact]
    public async void CreateTask_ReturnsCreatedAt()
    {
        var taskPayload = new TaskPayload { Title = "New Task", Assignees = new List<TaskController.AssigneePayload>() };
        var createdTask = new models.Task { Id = 3, Title = "New Task" };

        _mockService.Setup(s => s.CreateTaskAsync(taskPayload)).ReturnsAsync(createdTask);

        var result = await _controller.CreateTask(taskPayload);

        var createdAt = Assert.IsType<CreatedResult>(result);
        var returnedValue = Assert.IsType<models.Task>(createdAt.Value);
        Assert.Equal(createdTask.Id, returnedValue.Id);
    }

    [Fact]
    public async void CreateTask_ReturnsBadRequest_WhenPayloadIsNull()
    { 
        var result = await _controller.CreateTask(null);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequest.StatusCode);
    }
}
