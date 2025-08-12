using Dapper;

namespace Todo.Infrastructure.DataAccess;

using System.Data;

public class DapperExecutor : IDapperExecutor
{
    private readonly IDbConnection _connection;

    /// <inheritdoc />
    public DapperExecutor(IDbConnection connection)
    {
        _connection = connection;
    }

    /// <inheritdoc />
    public Task<IEnumerable<T>> QueryAsync<T>(CommandDefinition command)
    {
        return _connection.QueryAsync<T>(command);
    }

    /// <inheritdoc />
    public Task<T?> QueryFirstOrDefaultAsync<T>(CommandDefinition command)
    {
        return _connection.QueryFirstOrDefaultAsync<T>(command);
    }

    /// <inheritdoc />
    public Task<int> ExecuteAsync(CommandDefinition command)
    {
        return _connection.ExecuteAsync(command);
    }

    /// <inheritdoc />
    public Task<T?> ExecuteScalarAsync<T>(CommandDefinition command)
    {
        return _connection.ExecuteScalarAsync<T>(command);
    }
}