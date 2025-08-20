using FluentResults;
using Todo.Application.Models.User;

namespace Todo.Application.UseCases.Users.GetUsers;

public interface IGetUsersUseCase
{
    /// <summary>
    /// Executes the asynchronous.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="ct">The <see cref="CancellationToken"/>.</param>
    /// <returns></returns>
    public Task<Result<IEnumerable<UserDto>>> HandleAsync(GetUsersQuery query, CancellationToken ct = default);
}