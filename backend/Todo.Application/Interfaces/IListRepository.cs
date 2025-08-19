namespace Todo.Application.Interfaces;

using Domain.Entities;

public interface IListRepository
{
    /// <summary>
    /// Retrieves a <see cref="TodoList"/> by its unique identifier asynchronously.
    /// </summary>
    /// <param name="id">The <see cref="TodoList.Id"/> to locate.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// The matching <see cref="TodoList"/> or <c>null</c> if no list with the specified <paramref name="id"/> exists.
    /// </returns>
    Task<TodoList?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all <see cref="TodoList"/>s that belong to a specific user, with paging support.
    /// </summary>
    /// <param name="userId">The <see cref="TodoList.OwnerId"/> of the owner.</param>
    /// <param name="skip">Number of records to skip for pagination.</param>
    /// <param name="take">Maximum number of records to return.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// An <see cref="IEnumerable{TodoList}"/> containing the requested page of lists for the user.
    /// </returns>
    Task<IEnumerable<TodoList>> GetForUserAsync(Guid userId, int skip, int take, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new <see cref="TodoList"/> to the store asynchronously.
    /// </summary>
    /// <param name="list">The list entity to persist.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task AddAsync(TodoList list, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing <see cref="TodoList"/> in the store asynchronously.
    /// </summary>
    /// <param name="list">The list entity with updated values.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task UpdateAsync(TodoList list, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a <see cref="TodoList"/> from the store asynchronously.
    /// </summary>
    /// <param name="list">The list entity to delete.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task DeleteAsync(TodoList list, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new share association for a list asynchronously.
    /// </summary>
    /// <param name="share">The <see cref="TodoListShare"/> entity to add.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task AddShareAsync(TodoListShare share, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes an existing share association for a list asynchronously.
    /// </summary>
    /// <param name="share">The <see cref="TodoListShare"/> entity to remove.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task RemoveShareAsync(TodoListShare share, CancellationToken cancellationToken = default);
}