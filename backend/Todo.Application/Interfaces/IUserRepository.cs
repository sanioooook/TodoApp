namespace Todo.Application.Interfaces;

using Domain.Entities;

public interface IUserRepository
{
    /// <summary>
    /// Gets the by identifier asynchronous.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="ct">The ct.</param>
    /// <returns>user</returns>
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Gets the by email asynchronous.
    /// </summary>
    /// <param name="email">The email.</param>
    /// <param name="ct">The ct.</param>
    /// <exception cref="ArgumentException"> if email is null or empty or have only whitespaces </exception>
    /// <returns>user</returns>
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);

    /// <summary>
    /// Gets all asynchronous.
    /// </summary>
    /// <param name="skip">The skip.</param>
    /// <param name="take">The take.</param>
    /// <param name="ct">The ct.</param>
    /// <returns>all users</returns>
    Task<IEnumerable<User>> GetAllAsync(int skip, int take, CancellationToken ct = default);

    /// <summary>
    /// Adds the asynchronous.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="ct">The ct.</param>
    /// <exception cref="ArgumentNullException"> if user is null </exception>
    /// <returns>true if created successfully; otherwise false</returns>
    Task<bool> AddAsync(User user, CancellationToken ct = default);

    /// <summary>
    /// Updates the asynchronous.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="ct">The ct.</param>
    /// <exception cref="ArgumentNullException">if user is null</exception>
    /// <returns>true if updated successfully; otherwise false</returns>
    Task<bool> UpdateAsync(User user, CancellationToken ct = default);

    /// <summary>
    /// Deletes the asynchronous.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="ct">The ct.</param>
    /// <returns>true if deleted successfully; otherwise false</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}