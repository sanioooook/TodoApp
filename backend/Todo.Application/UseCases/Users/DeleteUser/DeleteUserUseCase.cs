namespace Todo.Application.UseCases.Users.DeleteUser;

using Enums;
using Interfaces;

public class DeleteUserUseCase : IDeleteUserUseCase
{
    private readonly IUserRepository _userRepository;

    public DeleteUserUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    /// <inheritdoc />
    public async Task<DeleteUserResult> ExecuteAsync(DeleteUserCommand command, CancellationToken ct = default)
    {
        var existing = await _userRepository.GetByIdAsync(command.Id, ct);
        if (existing == null)
            return new DeleteUserResult { Success = false, Message = "User not found", CodeResult = ResultCode.NotFound };

        await _userRepository.DeleteAsync(existing, ct);
        return new DeleteUserResult { Success = true, Message = "User successfully deleted", CodeResult = ResultCode.Success };
    }
}