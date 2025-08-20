namespace Todo.Application.Tests.UseCases.TodoLists;

using Domain.Entities;
using FluentAssertions;
using Interfaces;
using Moq;
using Todo.Application.UseCases.TodoLists.UnshareTodoList;

public class UnshareTodoListUseCaseTests
{
    private readonly Mock<IListRepository> _repoMock;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly UnshareTodoListUseCase _useCase;

    public UnshareTodoListUseCaseTests()
    {
        _repoMock = new Mock<IListRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _useCase = new UnshareTodoListUseCase(_repoMock.Object, _mockUserRepository.Object);
    }
    [Fact]
    public async Task Should_Return_NotFound_When_List_Does_Not_Exist()
    {
        var command = new UnshareTodoListCommand { ListId = Guid.NewGuid(), TargetUserId = Guid.NewGuid(), CurrentUserId = Guid.NewGuid() };

        _repoMock.Setup(r => r.GetByIdAsync(command.ListId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TodoList?)null);

        var result = await _useCase.HandleAsync(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors.First().Metadata["ErrorCode"].Should().Be("NotFound");
    }

    [Fact]
    public async Task Should_Remove_Share_When_Owner()
    {
        var ownerId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();

        var command = new UnshareTodoListCommand
        {
            ListId = Guid.NewGuid(),
            TargetUserId = targetUserId,
            CurrentUserId = ownerId
        };

        var list = new TodoList
        {
            Id = command.ListId,
            OwnerId = ownerId,
            Title = "Shared",
            Shares = new List<TodoListShare>
            {
                new TodoListShare { TodoListId = command.ListId, UserId = targetUserId }
            }
        };

        _repoMock.Setup(r => r.GetByIdAsync(command.ListId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);

        _repoMock.Setup(r => r.RemoveShareAsync(It.IsAny<TodoListShare>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockUserRepository
            .Setup(r => r.GetByIdAsync(targetUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = targetUserId, Email = "target@example.com", FullName = "Target User" });

        var result = await _useCase.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _repoMock.Verify(r => r.RemoveShareAsync(It.IsAny<TodoListShare>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}