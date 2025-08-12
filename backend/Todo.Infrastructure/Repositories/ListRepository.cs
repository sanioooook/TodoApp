namespace Todo.Infrastructure.Repositories;

using Application.Interfaces;
using Dapper;
using DataAccess;
using Domain.Entities;

public class ListRepository : IListRepository
{
    private readonly IDapperExecutor _executor;

    public ListRepository(IDapperExecutor executor)
    {
        _executor = executor;
    }

    public async Task<TodoList?> GetByIdAsync(Guid id, Guid userId, CancellationToken ct = default)
    {
        var sql = @"
        SELECT tl.* FROM todo_lists tl
        WHERE tl.id = @id AND (
            tl.owner_id = @userId OR
            EXISTS (
                SELECT 1
                FROM todo_list_shares tls
                WHERE tls.todo_list_id = tl.id AND tls.user_id = @userId
            )
        )";
        var list = await _executor.QueryFirstOrDefaultAsync<TodoList>(
            new CommandDefinition(sql, new { id, userId }, cancellationToken: ct));

        if (list == null)
            return null;

        var sharesSql = "SELECT * FROM todo_list_shares WHERE todo_list_id = @id";
        var shares = await _executor.QueryAsync<TodoListShare>(
            new CommandDefinition(sharesSql, new { id }, cancellationToken: ct));

        list.Shares = shares.ToList();
        return list;
    }

    public async Task<IEnumerable<TodoList>> GetForUserAsync(Guid userId, int skip, int take,
        CancellationToken ct = default)
    {
        var sql = @"
        SELECT DISTINCT tl.* FROM todo_lists tl
        LEFT JOIN todo_list_shares tls ON tls.todo_list_id = tl.id
        WHERE tl.owner_id = @userId OR tls.user_id = @userId
        ORDER BY tl.created_at DESC
        OFFSET @skip LIMIT @take";

        var lists = await _executor.QueryAsync<TodoList>(
            new CommandDefinition(sql, new { userId, skip, take }, cancellationToken: ct));

        return lists;
    }

    public async Task<int> CountSharesAsync(Guid listId, CancellationToken ct = default)
    {
        var sql = "SELECT COUNT(*) FROM todo_list_shares WHERE todo_list_id = @listId";
        return await _executor.ExecuteScalarAsync<int>(new CommandDefinition(sql, new { listId }, cancellationToken: ct));
    }

    public async Task<bool> AddAsync(TodoList list, CancellationToken ct = default)
    {
        if (list is null)
            throw new ArgumentNullException(nameof(list));

        list.CreatedAt = DateTime.UtcNow;
        list.UpdatedAt = DateTime.UtcNow;

        var sql = @"INSERT INTO todo_lists (id, title, owner_id, created_at, updated_at)
                        VALUES (@Id, @Title, @OwnerId, @CreatedAt, @UpdatedAt)";
        var affectedRows = await _executor.ExecuteAsync(new CommandDefinition(sql, list, cancellationToken: ct));
        return affectedRows > 0;
    }

    public async Task<bool> UpdateAsync(TodoList list, CancellationToken ct = default)
    {
        if (list is null)
            throw new ArgumentNullException(nameof(list));

        list.UpdatedAt = DateTime.UtcNow;

        var sql = @"UPDATE todo_lists
                        SET title = @Title, updated_at = @UpdatedAt
                        WHERE id = @Id";
        var affectedRows = await _executor.ExecuteAsync(new CommandDefinition(sql, list, cancellationToken: ct));
        return affectedRows > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var sql = "DELETE FROM todo_lists WHERE id = @id";
        var affectedRows = await _executor.ExecuteAsync(new CommandDefinition(sql, new { id }, cancellationToken: ct));
        return affectedRows > 0;
    }

    public async Task<bool> AddShareAsync(Guid listId, Guid userId, CancellationToken ct = default)
    {
        var sql = @"INSERT INTO todo_list_shares (id, todo_list_id, user_id, created_at)
                        VALUES (@Id, @ListId, @UserId, @CreatedAt)";
        var affectedRows = await _executor.ExecuteAsync(new CommandDefinition(sql,
            new { Id = Guid.NewGuid(), ListId = listId, UserId = userId, CreatedAt = DateTime.UtcNow },
            cancellationToken: ct));
        return affectedRows > 0;
    }

    public async Task<bool> RemoveShareAsync(Guid listId, Guid userId, CancellationToken ct = default)
    {
        var sql = "DELETE FROM todo_list_shares WHERE todo_list_id = @listId AND user_id = @userId";
        var affectedRows =
            await _executor.ExecuteAsync(new CommandDefinition(sql, new { listId, userId }, cancellationToken: ct));
        return affectedRows > 0;
    }

    public async Task<IEnumerable<TodoListShare>> GetSharesAsync(Guid listId, CancellationToken ct = default)
    {
        var sql = "SELECT * FROM todo_list_shares WHERE todo_list_id = @listId";
        return await _executor.QueryAsync<TodoListShare>(new CommandDefinition(sql, new { listId }, cancellationToken: ct));
    }
}