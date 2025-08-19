namespace Todo.Infrastructure.Tests.Repositories;

using Domain.Entities;
using Microsoft.EntityFrameworkCore;

public abstract class RepositoryTestBase : IDisposable
{
    protected readonly TodoDbContext Context;

    protected RepositoryTestBase()
    {
        var options = new DbContextOptionsBuilder<TodoDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        Context = new TodoDbContext(options);
    }

    public void Dispose()
    {
        Context.Dispose();
    }
    protected async Task<User> CreateTestUserAsync(string email = "test@example.com", string fullName = "Test User")
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            FullName = fullName,
            CreatedAt = DateTime.UtcNow
        };

        Context.Users.Add(user);
        await Context.SaveChangesAsync();
        return user;
    }

    protected async Task<TodoList> CreateTestTodoListAsync(Guid ownerId, string title = "Test List")
    {
        var list = new TodoList
        {
            Id = Guid.NewGuid(),
            Title = title,
            OwnerId = ownerId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        Context.TodoLists.Add(list);
        await Context.SaveChangesAsync();
        return list;
    }
}