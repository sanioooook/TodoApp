namespace Todo.Application.UseCases.Users.DeleteUser;

using Common;
using FluentResults;
using Interfaces;

public class DeleteUserUseCase(IUserRepository userRepository) : IDeleteUserUseCase
{
    /// <inheritdoc />
    public async Task<Result> ExecuteAsync(DeleteUserCommand command, CancellationToken ct = default)
    {
        var existing = await userRepository.GetByIdAsync(command.Id, ct);
        if (existing == null)
            return Result.Fail(Errors.NotFound("User", command.Id));

        await userRepository.DeleteAsync(existing, ct);
        return Result.Ok();
    }
}