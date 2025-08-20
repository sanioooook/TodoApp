namespace Todo.Application.Tests.UseCases.Users;

using Domain.Entities;
using FluentAssertions;
using Interfaces;
using Moq;
using Todo.Application.UseCases.Users.GetUserById;

public class GetUserByIdUseCaseTests
{
    private readonly Mock<IUserRepository> _repoMock;
    private readonly GetUserByIdUseCase _useCase;

    public GetUserByIdUseCaseTests()
    {
        _repoMock = new Mock<IUserRepository>();
        _useCase = new GetUserByIdUseCase(_repoMock.Object);
    }

    [Fact]
    public async Task Should_Return_User_When_Found()
    {
        var user = new User { Id = Guid.NewGuid(), Email = "found@example.com", FullName = "Found User" };

        _repoMock.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _useCase.HandleAsync(new GetUserByIdQuery { Id = user.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Email.Should().Be("found@example.com");
    }

    [Fact]
    public async Task Should_Return_NotFound_When_User_Does_Not_Exist()
    {
        var id = Guid.NewGuid();

        _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var result = await _useCase.HandleAsync(new GetUserByIdQuery { Id = id }, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors.First().Metadata["ErrorCode"].Should().Be("NotFound");
    }
}