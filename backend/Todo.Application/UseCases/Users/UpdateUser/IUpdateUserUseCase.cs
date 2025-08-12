namespace Todo.Application.UseCases.Users.UpdateUser;

public interface IUpdateUserUseCase
{
    /// <summary>
    /// Executes the asynchronous.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="ct">The <see cref="CancellationToken"/>.</param>
    /// <returns></returns>
    public Task<UpdateUserResult> ExecuteAsync(UpdateUserCommand command, CancellationToken ct = default);
}