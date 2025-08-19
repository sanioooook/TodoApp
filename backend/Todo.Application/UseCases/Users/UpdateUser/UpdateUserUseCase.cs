namespace Todo.Application.UseCases.Users.UpdateUser;

using Enums;
using Interfaces;

public class UpdateUserUseCase : IUpdateUserUseCase
{
    private readonly IUserRepository _userRepository;

    public UpdateUserUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    /// <inheritdoc />
    public async Task<UpdateUserResult> ExecuteAsync(UpdateUserCommand command, CancellationToken ct = default)
    {
        var existing = await _userRepository.GetByIdAsync(command.Id, ct);
        if (existing == null)
            return new UpdateUserResult { Success = false, Message = "User not found", CodeResult = ResultCode.NotFound };

        // check for email conflict if changed
        if (!string.Equals(existing.Email, command.Email, StringComparison.OrdinalIgnoreCase))
        {
            var conflict = await _userRepository.GetByEmailAsync(command.Email, ct);
            if (conflict != null)
                return new UpdateUserResult
                    { Success = false, Message = "Email is already in use", CodeResult = ResultCode.Conflict };
        }

        existing.Email = command.Email;
        existing.FullName = command.FullName;

        await _userRepository.UpdateAsync(existing, ct);

        return new UpdateUserResult { Success = true, Message = "Successfully updated User", CodeResult = ResultCode.Success };
    }
}