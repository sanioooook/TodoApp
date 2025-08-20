namespace Todo.Application.Tests.UseCases.Users;

using Domain.Entities;
using FluentAssertions;
using Interfaces;
using Moq;
using Todo.Application.UseCases.Users.CreateUser;

public class CreateUserUseCaseTests
{
    private readonly Mock<IUserRepository> _repoMock;
    private readonly CreateUserUseCase _useCase;

    public CreateUserUseCaseTests()
    {
        _repoMock = new Mock<IUserRepository>();
        _useCase = new CreateUserUseCase(_repoMock.Object);
    }

    [Fact]
    public async Task Should_Create_User()
    {
        var command = new CreateUserCommand { Email = "new@example.com", FullName = "New User" };

        var result = await _useCase.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Email.Should().Be("new@example.com");
        _repoMock.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}