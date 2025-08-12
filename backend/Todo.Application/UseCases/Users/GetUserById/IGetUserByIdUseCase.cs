namespace Todo.Application.UseCases.Users.GetUserById;

public interface IGetUserByIdUseCase
{
    /// <summary>
    /// Executes the asynchronous.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="ct">The <see cref="CancellationToken"/>.</param>
    /// <returns></returns>
    public Task<GetUserByIdResult> ExecuteAsync(GetUserByIdQuery query, CancellationToken ct = default);
}