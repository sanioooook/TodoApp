namespace Todo.Application.Tests.UseCases.TodoLists;

using FluentAssertions;
using Domain.Entities;
using Interfaces;
using Moq;
using Todo.Application.UseCases.TodoLists.UpdateTodoList;

public class UpdateTodoListUseCaseTests
{
    private readonly Mock<IListRepository> _repoMock;
    private readonly UpdateTodoListUseCase _useCase;

    public UpdateTodoListUseCaseTests()
    {
        _repoMock = new Mock<IListRepository>();
        _useCase = new UpdateTodoListUseCase(_repoMock.Object);
    }

    [Fact]
    public async Task Should_Return_NotFound_When_List_Does_Not_Exist()
    {
        var command = new UpdateTodoListCommand { Id = Guid.NewGuid(), Title = "Updated", CurrentUserId = Guid.NewGuid() };

        _repoMock.Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TodoList?)null);

        var result = await _useCase.HandleAsync(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors.First().Metadata["ErrorCode"].Should().Be("NotFound");
    }

    [Fact]
    public async Task Should_Update_List_When_User_Is_Owner()
    {
        var userId = Guid.NewGuid();
        var command = new UpdateTodoListCommand { Id = Guid.NewGuid(), Title = "Updated", CurrentUserId = userId };

        var list = new TodoList { Id = command.Id, OwnerId = userId, Title = "Old" };

        _repoMock.Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);

        var result = await _useCase.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        list.Title.Should().Be("Updated");
        _repoMock.Verify(r => r.UpdateAsync(list, It.IsAny<CancellationToken>()), Times.Once);
    }
}