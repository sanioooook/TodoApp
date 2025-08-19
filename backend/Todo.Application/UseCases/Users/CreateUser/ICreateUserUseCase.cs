namespace Todo.Application.UseCases.Users.CreateUser;

using FluentResults;
using Models;

public interface ICreateUserUseCase
{
    /// <summary>
    /// Executes the asynchronous.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="ct">The <see cref="CancellationToken"/>.</param>
    /// <returns></returns>
    public Task<Result<UserDto>> ExecuteAsync(CreateUserCommand command, CancellationToken ct = default);
}