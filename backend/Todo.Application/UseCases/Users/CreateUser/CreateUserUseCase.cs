namespace Todo.Application.UseCases.Users.CreateUser;

using Domain.Entities;
using Enums;
using Interfaces;

public class CreateUserUseCase : ICreateUserUseCase
{
    private readonly IUserRepository _userRepository;

    public CreateUserUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    /// <inheritdoc />
    public async Task<CreateUserResult> ExecuteAsync(CreateUserCommand command, CancellationToken ct = default)
    {
        // Check if email already exists
        var existing = await _userRepository.GetByEmailAsync(command.Email, ct);
        if (existing != null)
            return new CreateUserResult { Success = false, Message = $"User with email {command.Email} has already created", CodeResult = ResultCode.Conflict};

        var user = new User
        {
            Email = command.Email,
            FullName = command.FullName,
        };

        await _userRepository.AddAsync(user, ct);
        return new CreateUserResult { Id = user.Id, Success = true, Message = "Successfully created User", CodeResult = ResultCode.Success};
    }
}