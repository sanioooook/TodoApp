namespace Todo.Application.Tests.UseCases.Users;

using Domain.Entities;
using FluentAssertions;
using Interfaces;
using Moq;
using Todo.Application.UseCases.Users.DeleteUser;

public class DeleteUserUseCaseTests
{
    private readonly Mock<IUserRepository> _repoMock;
    private readonly DeleteUserUseCase _useCase;

    public DeleteUserUseCaseTests()
    {
        _repoMock = new Mock<IUserRepository>();
        _useCase = new DeleteUserUseCase(_repoMock.Object);
    }

    [Fact]
    public async Task Should_Return_NotFound_When_User_Does_Not_Exist()
    {
        var command = new DeleteUserCommand { Id = Guid.NewGuid() };

        _repoMock.Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var result = await _useCase.HandleAsync(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors.First().Metadata["ErrorCode"].Should().Be("NotFound");
    }

    [Fact]
    public async Task Should_Delete_User_When_Exists()
    {
        var user = new User { Id = Guid.NewGuid(), Email = "delete@example.com", FullName = "Delete Me" };

        var command = new DeleteUserCommand { Id = user.Id };

        _repoMock.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _useCase.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _repoMock.Verify(r => r.DeleteAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }
}