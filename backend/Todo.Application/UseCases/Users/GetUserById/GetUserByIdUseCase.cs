namespace Todo.Application.UseCases.Users.GetUserById;

using Common;
using FluentResults;
using Interfaces;
using Models.User;

public class GetUserByIdUseCase(IUserRepository userRepository) : IGetUserByIdUseCase
{
    /// <inheritdoc />
    public async Task<Result<UserDto>> ExecuteAsync(GetUserByIdQuery query, CancellationToken ct = default)
    {
        var user = await userRepository.GetByIdAsync(query.Id, ct);
        if (user is null)
            return Result.Fail(Errors.NotFound("User", query.Id));

        return Result.Ok(new UserDto
        {
            Email = user.Email,
            FullName = user.FullName,
            Id = user.Id,
            CreatedAt = user.CreatedAt,
        });
        
    }
}