namespace Todo.Application.UseCases.Users.CreateUser;

using Common;
using Domain.Entities;
using FluentResults;
using Interfaces;
using Models.User;

public class CreateUserUseCase(IUserRepository userRepository) : ICreateUserUseCase
{
    /// <inheritdoc />
    public async Task<Result<UserDto>> HandleAsync(CreateUserCommand command, CancellationToken ct = default)
    {
        // Check if email already exists
        var existing = await userRepository.GetByEmailAsync(command.Email, ct);
        if (existing != null)
            return Result.Fail(Errors.Conflict($"User with email {command.Email} has already created"));

        var user = new User
        {
            Email = command.Email,
            FullName = command.FullName,
        };

        await userRepository.AddAsync(user, ct);
        return Result.Ok(new UserDto
        {
            CreatedAt = user.CreatedAt,
            Email = user.Email,
            FullName = user.FullName,
            Id = user.Id,
        });
    }
}