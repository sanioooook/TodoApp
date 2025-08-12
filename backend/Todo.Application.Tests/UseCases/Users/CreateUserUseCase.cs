namespace Todo.Application.Tests.UseCases.Users;

using Domain.Entities;
using Enums;
using Interfaces;
using Moq;
using Todo.Application.UseCases.Users.CreateUser;

public class CreateUserUseCaseTests
{
    private readonly Mock<IUserRepository> _mockRepository;
    private readonly CreateUserUseCase _useCase;

    public CreateUserUseCaseTests()
    {
        _mockRepository = new Mock<IUserRepository>();
        _useCase = new CreateUserUseCase(_mockRepository.Object);
    }

    [Fact]
    public async Task CreateUserUseCaseTests_CreateUserWithValidData_ReturnsSuccess()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            Email = "test@example.com",
            FullName = "Test User"
        };

        _mockRepository.Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                      .ReturnsAsync((User)null);

        _mockRepository.Setup(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(true);

        // Act
        var result = await _useCase.ExecuteAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(ResultCode.Success, result.CodeResult);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Contains("Successfully created User", result.Message);
    }

    [Fact]
    public async Task CreateUserUseCaseTests_CreateUserWithExistingEmail_ReturnsConflict()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            Email = "existing@example.com",
            FullName = "Test User"
        };

        var existingUser = new User
        {
            Id = Guid.NewGuid(),
            Email = command.Email,
            FullName = "Existing User"
        };

        _mockRepository.Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(existingUser);

        // Act
        var result = await _useCase.ExecuteAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultCode.Conflict, result.CodeResult);
        Assert.Contains("User with email", result.Message);
    }

    [Fact]
    public async Task CreateUserUseCaseTests_FailedToCreateUser_ReturnsError()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            Email = "test@example.com",
            FullName = "Test User"
        };

        _mockRepository.Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                      .ReturnsAsync((User)null);

        _mockRepository.Setup(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(false);

        // Act
        var result = await _useCase.ExecuteAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultCode.ServerError, result.CodeResult);
        Assert.Contains("Error while creating User", result.Message);
    }
}