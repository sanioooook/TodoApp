namespace Todo.Application.Tests.UseCases.TodoLists;

using Domain.Entities;
using FluentAssertions;
using Interfaces;
using Moq;
using Todo.Application.UseCases.TodoLists.DeleteTodoList;

public class DeleteTodoListUseCaseTests
{
    private readonly Mock<IListRepository> _repoMock;
    private readonly DeleteTodoListUseCase _useCase;

    public DeleteTodoListUseCaseTests()
    {
        _repoMock = new Mock<IListRepository>();
        _useCase = new DeleteTodoListUseCase(_repoMock.Object);
    }

    [Fact]
    public async Task Should_Return_NotFound_When_List_Does_Not_Exist()
    {
        var command = new DeleteTodoListCommand { Id = Guid.NewGuid(), CurrentUserId = Guid.NewGuid() };

        _repoMock.Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TodoList?)null);

        var result = await _useCase.HandleAsync(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors.First().Metadata["ErrorCode"].Should().Be("NotFound");
    }

    [Fact]
    public async Task Should_Return_Unauthorized_When_User_Is_Not_Owner()
    {
        var command = new DeleteTodoListCommand { Id = Guid.NewGuid(), CurrentUserId = Guid.NewGuid() };
        var list = new TodoList { Id = command.Id, OwnerId = Guid.NewGuid(), Title = "Test" };

        _repoMock.Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);

        var result = await _useCase.HandleAsync(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors.First().Metadata["ErrorCode"].Should().Be("Unauthorized");
    }

    [Fact]
    public async Task Should_Delete_When_User_Is_Owner()
    {
        var userId = Guid.NewGuid();
        var command = new DeleteTodoListCommand { Id = Guid.NewGuid(), CurrentUserId = userId };
        var list = new TodoList { Id = command.Id, OwnerId = userId, Title = "Test" };

        _repoMock.Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);

        var result = await _useCase.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _repoMock.Verify(r => r.DeleteAsync(list, It.IsAny<CancellationToken>()), Times.Once);
    }
}