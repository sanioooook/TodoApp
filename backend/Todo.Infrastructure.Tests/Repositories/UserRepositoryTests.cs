namespace Todo.Infrastructure.Tests.Repositories;

using Domain.Entities;
using FluentAssertions;
using Todo.Infrastructure.Repositories;

public class UserRepositoryTests : RepositoryTestBase
{
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        _repository = new UserRepository(Context);
    }

    [Fact]
    public async Task AddAsync_ShouldAddUser()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "new@example.com",
            FullName = "New User",
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(user);

        var dbUser = await Context.Users.FindAsync(user.Id);
        dbUser.Should().NotBeNull();
        dbUser!.Email.Should().Be("new@example.com");
        dbUser!.FullName.Should().Be("New User");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUser()
    {
        var user = await CreateTestUserAsync();

        var result = await _repository.GetByIdAsync(user.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnUser()
    {
        await CreateTestUserAsync("lookup@example.com");

        var result = await _repository.GetByEmailAsync("lookup@example.com");

        result.Should().NotBeNull();
        result!.Email.Should().Be("lookup@example.com");
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateUser()
    {
        var user = await CreateTestUserAsync();
        user.FullName = "Updated User";

        await _repository.UpdateAsync(user);

        var dbUser = await Context.Users.FindAsync(user.Id);
        dbUser!.FullName.Should().Be("Updated User");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveUser()
    {
        var user = await CreateTestUserAsync();

        await _repository.DeleteAsync(user);

        var dbUser = await Context.Users.FindAsync(user.Id);
        dbUser.Should().BeNull();
    }
}
