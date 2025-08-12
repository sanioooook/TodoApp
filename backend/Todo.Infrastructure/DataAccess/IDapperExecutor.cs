namespace Todo.Infrastructure.DataAccess;

using Dapper;

public interface IDapperExecutor
{
    // Executes a query and returns a sequence of results
    Task<IEnumerable<T>> QueryAsync<T>(CommandDefinition command);

    // Executes a single-row query and returns one or default
    Task<T?> QueryFirstOrDefaultAsync<T>(CommandDefinition command);

    // Executes a command (INSERT, UPDATE, DELETE) and returns affected rows count
    Task<int> ExecuteAsync(CommandDefinition command);

    Task<T?> ExecuteScalarAsync<T>(CommandDefinition commandDefinition);
}
