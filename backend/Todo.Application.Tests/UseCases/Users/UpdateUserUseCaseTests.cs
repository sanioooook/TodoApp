namespace Todo.Application.Tests.UseCases.Users;

using Domain.Entities;
using FluentAssertions;
using Interfaces;
using Moq;
using Todo.Application.UseCases.Users.UpdateUser;

public class UpdateUserUseCaseTests
{
    private readonly Mock<IUserRepository> _repoMock;
    private readonly UpdateUserUseCase _useCase;

    public UpdateUserUseCaseTests()
    {
        _repoMock = new Mock<IUserRepository>();
        _useCase = new UpdateUserUseCase(_repoMock.Object);
    }

    [Fact]
    public async Task Should_Return_NotFound_When_User_Does_Not_Exist()
    {
        var command = new UpdateUserCommand { Id = Guid.NewGuid(), Email = "missing@example.com", FullName = "Updated User" };

        _repoMock.Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var result = await _useCase.HandleAsync(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors.First().Metadata["ErrorCode"].Should().Be("NotFound");
    }

    [Fact]
    public async Task Should_Update_User_When_Exists()
    {
        var user = new User { Id = Guid.NewGuid(), Email = "old@example.com", FullName = "Old Name" };

        var command = new UpdateUserCommand { Id = user.Id, Email = "new@example.com", FullName = "Updated User" };

        _repoMock.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _useCase.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        user.FullName.Should().Be("Updated User");
        user.Email.Should().Be("new@example.com");

        _repoMock.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }
}