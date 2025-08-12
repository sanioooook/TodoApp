using Todo.Application.Enums;

namespace Todo.Application.Tests.UseCases.TodoLists;

using Domain.Entities;
using Interfaces;
using Moq;
using Todo.Application.UseCases.TodoLists.CreateTodoList;

public class CreateTodoListUseCaseTests
{
    private readonly Mock<IListRepository> _mockRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly CreateTodoListUseCase _useCase;

    public CreateTodoListUseCaseTests()
    {
        _mockRepository = new Mock<IListRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _useCase = new CreateTodoListUseCase(_mockRepository.Object, _mockUserRepository.Object);
    }

    [Fact]
    public async Task CreateTodoListUseCaseTests_CreateWithValidTitle_ReturnsSuccess()
    {
        // Arrange
        var command = new CreateTodoListCommand
        {
            Title = "Valid Title",
            OwnerId = Guid.NewGuid()
        };

        var mockUser = new User
        {
            Id = command.OwnerId,
            Email = "test@example.com",
            FullName = "Test User"
        };

        _mockRepository.Setup(x => x.AddAsync(It.IsAny<TodoList>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockUserRepository.Setup(x => x.GetByIdAsync(command.OwnerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockUser);

        // Act
        var result = await _useCase.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(ResultCode.Success, result.CodeResult);
        Assert.NotEqual(Guid.Empty, result.TodoListDto.Id);
        Assert.Equal(command.Title, result.TodoListDto.Title);
        Assert.Equal(command.OwnerId, result.TodoListDto.OwnerId);
        Assert.NotEqual(DateTime.MinValue, result.TodoListDto.CreatedAt);
        Assert.NotEqual(DateTime.MinValue, result.TodoListDto.UpdatedAt);
    }

    [Fact]
    public async Task CreateTodoListUseCaseTests_CreateWithInvalidOwner_ReturnsError()
    {
        // Arrange
        var command = new CreateTodoListCommand
        {
            Title = "Valid Title",
            OwnerId = Guid.NewGuid()
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(command.OwnerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null);

        // Act
        var result = await _useCase.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultCode.BadRequest, result.CodeResult);
        Assert.Contains("OwnerId is wrong", result.Message);
    }

    [Fact]
    public async Task CreateTodoListUseCaseTests_CreateWithEmptyTitle_ReturnsError()
    {
        // Arrange
        var command = new CreateTodoListCommand
        {
            Title = "",
            OwnerId = Guid.NewGuid()
        };

        var mockUser = new User
        {
            Id = command.OwnerId,
            Email = "test@example.com",
            FullName = "Test User"
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(command.OwnerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockUser);

        // Act
        var result = await _useCase.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultCode.BadRequest, result.CodeResult);
        Assert.Contains("Title length need be more 1 symbol and less 255", result.Message);
    }

    [Fact]
    public async Task CreateTodoListUseCaseTests_CreateWithLongTitle_ReturnsError()
    {
        // Arrange
        var longTitle = new string('a', 256);
        var command = new CreateTodoListCommand
        {
            Title = longTitle,
            OwnerId = Guid.NewGuid()
        };

        var mockUser = new User
        {
            Id = command.OwnerId,
            Email = "test@example.com",
            FullName = "Test User"
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(command.OwnerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockUser);

        // Act
        var result = await _useCase.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultCode.BadRequest, result.CodeResult);
        Assert.Contains("Title length need be more 1 symbol and less 255", result.Message);
    }

    [Fact]
    public async Task CreateTodoListUseCaseTests_FailedToSave_ReturnsError()
    {
        // Arrange
        var command = new CreateTodoListCommand
        {
            Title = "Valid Title",
            OwnerId = Guid.NewGuid()
        };

        var mockUser = new User
        {
            Id = command.OwnerId,
            Email = "test@example.com",
            FullName = "Test User"
        };

        _mockRepository.Setup(x => x.AddAsync(It.IsAny<TodoList>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mockUserRepository.Setup(x => x.GetByIdAsync(command.OwnerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockUser);

        // Act
        var result = await _useCase.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultCode.ServerError, result.CodeResult);
        Assert.Contains("Error while creating TodoList", result.Message);
    }
}