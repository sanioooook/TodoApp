namespace Todo.Application.Tests.UseCases.Users;

using Moq;
using Todo.Application.Interfaces;
using Todo.Application.UseCases.Users.GetUsers;
using Todo.Domain.Entities;

public class GetUsersUseCaseTests
{
    private readonly Mock<IUserRepository> _mockRepository;
    private readonly GetUsersUseCase _useCase;

    public GetUsersUseCaseTests()
    {
        _mockRepository = new Mock<IUserRepository>();
        _useCase = new GetUsersUseCase(_mockRepository.Object);
    }

    [Fact]
    public async Task GetUsersUseCaseTests_GetEmptyList_ReturnsEmptyCollection()
    {
        // Arrange
        var query = new GetUsersQuery
        {
            Skip = 0,
            Take = 10
        };

        _mockRepository.Setup(x => x.GetAllAsync(query.Skip, query.Take, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(new List<User>());

        // Act
        var result = await _useCase.ExecuteAsync(query, CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUsersUseCaseTests_GetUsersWithPagination_ReturnsCorrectRange()
    {
        // Arrange
        var query = new GetUsersQuery
        {
            Skip = 1,
            Take = 2
        };

        // Создаем список из 5 элементов
        var allUsers = Enumerable.Range(1, 5).Select(i => new User
        {
            Id = Guid.NewGuid(),
            Email = $"user{i}@example.com",
            FullName = $"User {i}",
            CreatedAt = DateTime.UtcNow.AddDays(-i)
        }).ToList();

        // Репозиторий должен вернуть элементы со 2 по 3 (так как Skip=1, Take=2)
        var expectedUsers = allUsers.Skip(1).Take(2).ToList();

        _mockRepository.Setup(x => x.GetAllAsync(query.Skip, query.Take, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(expectedUsers);

        // Act
        var result = await _useCase.ExecuteAsync(query, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count());
        var resultList = result.ToList();
        Assert.Equal("user2@example.com", resultList[0].Email);
        Assert.Equal("user3@example.com", resultList[1].Email);
    }

    [Fact]
    public async Task GetUsersUseCaseTests_GetUsers_ReturnsCorrectData()
    {
        // Arrange
        var query = new GetUsersQuery
        {
            Skip = 0,
            Take = 10
        };

        var now = DateTime.UtcNow;
        var mockUsers = new List<User>
            {
                new User
                {
                    Id = Guid.NewGuid(),
                    Email = "user1@example.com",
                    FullName = "User One",
                    CreatedAt = now.AddDays(-2)
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Email = "user2@example.com",
                    FullName = "User Two",
                    CreatedAt = now.AddDays(-1)
                }
            };

        _mockRepository.Setup(x => x.GetAllAsync(query.Skip, query.Take, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(mockUsers);

        // Act
        var result = await _useCase.ExecuteAsync(query, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count());

        var resultList = result.ToList();
        Assert.Equal("user1@example.com", resultList[0].Email);
        Assert.Equal("user2@example.com", resultList[1].Email);
        Assert.Equal("User One", resultList[0].FullName);
        Assert.Equal("User Two", resultList[1].FullName);
    }
}