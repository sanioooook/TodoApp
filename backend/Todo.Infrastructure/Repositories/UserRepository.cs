namespace Todo.Infrastructure.Repositories;

using Application.Interfaces;
using Dapper;
using DataAccess;
using Domain.Entities;

public class UserRepository : IUserRepository
{
    private readonly IDapperExecutor _executor;

    public UserRepository(IDapperExecutor executor)
    {
        _executor = executor;
    }

    /// <inheritdoc />
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        const string sql = "SELECT * FROM users WHERE id = @Id";
        var user = await _executor.QueryFirstOrDefaultAsync<User>(new CommandDefinition(sql, new { Id = id },
            cancellationToken: ct));
        return user ?? null;
    }

    /// <inheritdoc />
    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException(nameof(email));
        const string sql = "SELECT * FROM users WHERE email = @email";
        var user = await _executor.QueryFirstOrDefaultAsync<User>(
            new CommandDefinition(sql, new { email }, cancellationToken: ct));

        return user ?? null;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<User>> GetAllAsync(int skip, int take, CancellationToken ct = default)
    {
        const string sql = @"SELECT *
                    FROM users
                    ORDER BY created_at DESC
                    OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY";
        var users = await _executor.QueryAsync<User>(
            new CommandDefinition(sql, new { Skip = skip, Take = take }, cancellationToken: ct));
        return users;
    }

    /// <inheritdoc />
    public async Task<bool> AddAsync(User user, CancellationToken ct = default)
    {
        if (user is null)
            throw new ArgumentNullException(nameof(user));

        user.CreatedAt = DateTime.UtcNow;

        const string sql = @"INSERT INTO users (id, email, full_name, created_at)
                             VALUES (@Id, @Email, @FullName, @CreatedAt)";
        var affectedRows = await _executor.ExecuteAsync(new CommandDefinition(sql, user, cancellationToken: ct));
        return affectedRows > 0;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(User user, CancellationToken ct = default)
    {
        if (user is null)
            throw new ArgumentNullException(nameof(user));

        const string sql = @"UPDATE users
                    SET email = @Email, full_name = @FullName
                    WHERE id = @Id";
        var affectedRows = await _executor.ExecuteAsync(new CommandDefinition(sql, user, cancellationToken: ct));
        return affectedRows > 0;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        const string sql = "DELETE FROM users WHERE id = @Id";
        var affectedRows = await _executor.ExecuteAsync(new CommandDefinition(sql, new { Id = id }, cancellationToken: ct));
        return affectedRows > 0;
    }
}