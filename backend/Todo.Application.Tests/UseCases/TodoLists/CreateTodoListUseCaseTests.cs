namespace Todo.Application.Tests.UseCases.TodoLists;

using FluentAssertions;
using Interfaces;
using Moq;
using Todo.Application.UseCases.TodoLists.CreateTodoList;
using Todo.Domain.Entities;

public class CreateTodoListUseCaseTests
{
    private readonly Mock<IListRepository> _repoMock;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly CreateTodoListUseCase _useCase;

    public CreateTodoListUseCaseTests()
    {
        _repoMock = new Mock<IListRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _useCase = new CreateTodoListUseCase(_repoMock.Object, _mockUserRepository.Object);
    }

    [Fact]
    public async Task Should_Create_List_When_Data_Is_Valid()
    {
        var ownerId = Guid.NewGuid();
        var command = new CreateTodoListCommand
        {
            Title = "My List",
            OwnerId = ownerId
        };

        _mockUserRepository
            .Setup(r => r.GetByIdAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = ownerId, Email = "owner@example.com", FullName = "Owner" });

        _repoMock
            .Setup(r => r.AddAsync(It.IsAny<TodoList>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _useCase.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Title.Should().Be("My List");
        result.Value.OwnerId.Should().Be(command.OwnerId);
    }
}