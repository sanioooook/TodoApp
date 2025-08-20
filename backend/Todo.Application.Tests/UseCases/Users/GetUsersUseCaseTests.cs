namespace Todo.Application.Tests.UseCases.Users;

using Domain.Entities;
using FluentAssertions;
using Interfaces;
using Moq;
using Todo.Application.UseCases.Users.GetUsers;

public class GetUsersUseCaseTests
{
    private readonly Mock<IUserRepository> _repoMock;
    private readonly GetUsersUseCase _useCase;

    public GetUsersUseCaseTests()
    {
        _repoMock = new Mock<IUserRepository>();
        _useCase = new GetUsersUseCase(_repoMock.Object);
    }

    [Fact]
    public async Task Should_Return_Paged_Users()
    {
        var users = new List<User>
        {
            new() { Id = Guid.NewGuid(), Email = "u1@example.com", FullName = "User 1" },
            new() { Id = Guid.NewGuid(), Email = "u2@example.com", FullName = "User 2" }
        };

        _repoMock.Setup(r => r.GetAllAsync(0, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        var result = await _useCase.HandleAsync(new GetUsersQuery { Skip = 0, Take = 10 }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }
}