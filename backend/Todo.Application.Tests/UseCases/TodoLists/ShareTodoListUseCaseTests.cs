namespace Todo.Application.Tests.UseCases.TodoLists;

using Domain.Entities;
using FluentAssertions;
using Interfaces;
using Moq;
using Todo.Application.UseCases.TodoLists.ShareTodoList;

public class ShareTodoListUseCaseTests
{
    private readonly Mock<IListRepository> _repoMock;
    private readonly ShareTodoListUseCase _useCase;

    public ShareTodoListUseCaseTests()
    {
        _repoMock = new Mock<IListRepository>();
        _useCase = new ShareTodoListUseCase(_repoMock.Object);
    }

    [Fact]
    public async Task Should_Return_NotFound_When_List_Does_Not_Exist()
    {
        var command = new ShareTodoListCommand { ListId = Guid.NewGuid(), TargetUserId = Guid.NewGuid(), CurrentUserId = Guid.NewGuid() };

        _repoMock.Setup(r => r.GetByIdAsync(command.ListId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TodoList?)null);

        var result = await _useCase.HandleAsync(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors.First().Metadata["ErrorCode"].Should().Be("NotFound");
    }

    [Fact]
    public async Task Should_Share_List_When_Owner()
    {
        var ownerId = Guid.NewGuid();
        var command = new ShareTodoListCommand { ListId = Guid.NewGuid(), TargetUserId = Guid.NewGuid(), CurrentUserId = ownerId };
        var list = new TodoList { Id = command.ListId, OwnerId = ownerId, Title = "Shared" };

        _repoMock.Setup(r => r.GetByIdAsync(command.ListId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);

        var result = await _useCase.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _repoMock.Verify(r => r.AddShareAsync(It.IsAny<TodoListShare>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}