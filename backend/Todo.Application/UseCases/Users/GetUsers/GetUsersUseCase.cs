namespace Todo.Application.UseCases.Users.GetUsers;

using FluentResults;
using Interfaces;
using Models.User;

public class GetUsersUseCase(IUserRepository userRepository) : IGetUsersUseCase
{
    /// <inheritdoc />
    public async Task<Result<IEnumerable<UserDto>>> ExecuteAsync(GetUsersQuery query, CancellationToken ct = default)
    {
        var users = await userRepository.GetAllAsync(query.Skip, query.Take, ct);
        return Result.Ok(users.Select(x => new UserDto
        {
            Id = x.Id,
            Email = x.Email,
            FullName = x.FullName,
            CreatedAt = x.CreatedAt,
        }));
    }
}