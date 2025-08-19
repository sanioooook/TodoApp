namespace Todo.Infrastructure.Tests.Repositories;

using FluentAssertions;
using Domain.Entities;
using Todo.Infrastructure.Repositories;

public class ListRepositoryTests : RepositoryTestBase
{
    private readonly ListRepository _repository;

    public ListRepositoryTests()
    {
        _repository = new ListRepository(Context);
    }

    [Fact]
    public async Task AddAsync_ShouldAddList()
    {
        var user = await CreateTestUserAsync();
        var list = new TodoList
        {
            Id = Guid.NewGuid(),
            Title = "New List",
            OwnerId = user.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(list);

        var dbList = await Context.TodoLists.FindAsync(list.Id);
        dbList.Should().NotBeNull();
        dbList!.Title.Should().Be("New List");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnListWithShares()
    {
        var user = await CreateTestUserAsync();
        var list = await CreateTestTodoListAsync(user.Id);

        var share = new TodoListShare
        {
            TodoListId = list.Id,
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow
        };

        Context.TodoListShares.Add(share);
        await Context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(list.Id);

        result.Should().NotBeNull();
        result!.Shares.Should().ContainSingle();
    }

    [Fact]
    public async Task GetForUserAsync_ShouldReturnOnlyUserLists()
    {
        var user1 = await CreateTestUserAsync("u1@example.com");
        var user2 = await CreateTestUserAsync("u2@example.com");

        var list1 = await CreateTestTodoListAsync(user1.Id, "List1");
        var list2 = await CreateTestTodoListAsync(user2.Id, "List2");

        var results = await _repository.GetForUserAsync(user1.Id, 0, 10);

        results.Should().ContainSingle()
            .Which.Title.Should().Be("List1");
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateList()
    {
        var user = await CreateTestUserAsync();
        var list = await CreateTestTodoListAsync(user.Id);
        list.Title = "Updated Title";

        await _repository.UpdateAsync(list);

        var dbList = await Context.TodoLists.FindAsync(list.Id);
        dbList!.Title.Should().Be("Updated Title");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveList()
    {
        var user = await CreateTestUserAsync();
        var list = await CreateTestTodoListAsync(user.Id);

        await _repository.DeleteAsync(list);

        var dbList = await Context.TodoLists.FindAsync(list.Id);
        dbList.Should().BeNull();
    }

    [Fact]
    public async Task AddShareAsync_ShouldAddShare()
    {
        var owner = await CreateTestUserAsync();
        var user = await CreateTestUserAsync("shared@example.com");
        var list = await CreateTestTodoListAsync(owner.Id);

        var share = new TodoListShare
        {
            TodoListId = list.Id,
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddShareAsync(share);

        var dbShare = await Context.TodoListShares.FindAsync(share.TodoListId, share.UserId);
        dbShare.Should().NotBeNull();
    }

    [Fact]
    public async Task RemoveShareAsync_ShouldRemoveShare()
    {
        var owner = await CreateTestUserAsync();
        var user = await CreateTestUserAsync("shared@example.com");
        var list = await CreateTestTodoListAsync(owner.Id);

        var share = new TodoListShare
        {
            TodoListId = list.Id,
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow
        };

        Context.TodoListShares.Add(share);
        await Context.SaveChangesAsync();

        await _repository.RemoveShareAsync(share);

        var dbShare = await Context.TodoListShares.FindAsync(share.TodoListId, share.UserId);
        dbShare.Should().BeNull();
    }
}
