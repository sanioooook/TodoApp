namespace Todo.Application.Tests.UseCases.Users;

using Domain.Entities;
using Enums;
using Interfaces;
using Moq;
using Todo.Application.UseCases.Users.GetUserById;

public class GetUserByIdUseCaseTests
{
    private readonly Mock<IUserRepository> _mockRepository;
    private readonly GetUserByIdUseCase _useCase;

    public GetUserByIdUseCaseTests()
    {
        _mockRepository = new Mock<IUserRepository>();
        _useCase = new GetUserByIdUseCase(_mockRepository.Object);
    }

    [Fact]
    public async Task GetUserByIdUseCaseTests_GetExistingUser_ReturnsSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var query = new GetUserByIdQuery
        {
            Id = userId
        };

        var mockUser = new User
        {
            Id = userId,
            Email = "test@example.com",
            FullName = "Test User",
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };

        _mockRepository.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(mockUser);

        // Act
        var result = await _useCase.ExecuteAsync(query, CancellationToken.None);

        // Assert
        Assert.True(result.HaveResult);
        Assert.Equal(ResultCode.Success, result.CodeResult);
        Assert.Equal(userId, result.User.Id);
        Assert.Equal(mockUser.Email, result.User.Email);
        Assert.Equal(mockUser.FullName, result.User.FullName);
        Assert.Equal(mockUser.CreatedAt, result.User.CreatedAt);
    }

    [Fact]
    public async Task GetUserByIdUseCaseTests_GetNonExistingUser_ReturnsNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var query = new GetUserByIdQuery
        {
            Id = userId
        };

        _mockRepository.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync((User)null);

        // Act
        var result = await _useCase.ExecuteAsync(query, CancellationToken.None);

        // Assert
        Assert.False(result.HaveResult);
        Assert.Equal(ResultCode.NotFound, result.CodeResult);
        Assert.Contains("User not found", result.Message);
    }
}