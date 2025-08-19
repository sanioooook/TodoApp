namespace Todo.Application.Interfaces;

using Domain.Entities;

public interface IUserRepository
{
    /// <summary>
    /// Retrieves a <see cref="User"/> by its unique identifier asynchronously.
    /// </summary>
    /// <param name="id">The <see cref="User.Id"/> to locate.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// The matching <see cref="User"/> or <c>null</c> if no user with the specified <paramref name="id"/> exists.
    /// </returns>
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a <see cref="User"/> by its email address asynchronously.
    /// </summary>
    /// <param name="email">The email to search for.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// The matching <see cref="User"/> or <c>null</c> if no user with the specified <paramref name="email"/> exists.
    /// </returns>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a page of <see cref="User"/> records asynchronously.
    /// </summary>
    /// <param name="skip">Number of records to skip for pagination.</param>
    /// <param name="take">Maximum number of records to return.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// An <see cref="IEnumerable{User}"/> containing the requested page of users.
    /// </returns>
    Task<IEnumerable<User>> GetAllAsync(int skip, int take, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new <see cref="User"/> to the store asynchronously.
    /// </summary>
    /// <param name="user">The user entity to persist.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task AddAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing <see cref="User"/> in the store asynchronously.
    /// </summary>
    /// <param name="user">The user entity with updated values.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a <see cref="User"/> from the store asynchronously.
    /// </summary>
    /// <param name="user">The user entity to delete.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task DeleteAsync(User user, CancellationToken cancellationToken = default);
}