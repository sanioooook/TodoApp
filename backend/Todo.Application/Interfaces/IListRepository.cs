namespace Todo.Application.Interfaces;

using Domain.Entities;

public interface IListRepository
{
    Task<TodoList?> GetByIdAsync(Guid id, Guid userId, CancellationToken ct = default);
    Task<IEnumerable<TodoList>> GetForUserAsync(Guid userId, int skip, int take, CancellationToken ct = default);
    Task<int> CountSharesAsync(Guid listId, CancellationToken ct = default);
    Task<bool> AddAsync(TodoList list, CancellationToken ct = default);
    Task<bool> UpdateAsync(TodoList list, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<bool> AddShareAsync(Guid listId, Guid userId, CancellationToken ct = default);
    Task<bool> RemoveShareAsync(Guid listId, Guid userId, CancellationToken ct = default);
    Task<IEnumerable<TodoListShare>> GetSharesAsync(Guid listId, CancellationToken ct = default);
}