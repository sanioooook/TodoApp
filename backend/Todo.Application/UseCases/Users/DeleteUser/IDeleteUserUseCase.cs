namespace Todo.Application.UseCases.Users.DeleteUser;

public interface IDeleteUserUseCase
{
    /// <summary>
    /// Executes the asynchronous.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="ct">The <see cref="CancellationToken"/>.</param>
    /// <returns></returns>
    public Task<DeleteUserResult> ExecuteAsync(DeleteUserCommand command, CancellationToken ct = default);
}