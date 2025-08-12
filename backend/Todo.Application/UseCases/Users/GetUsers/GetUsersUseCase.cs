namespace Todo.Application.UseCases.Users.GetUsers;

using Interfaces;

public class GetUsersUseCase: IGetUsersUseCase
{
    private readonly IUserRepository _userRepository;

    public GetUsersUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<GetUsersResult>> ExecuteAsync(GetUsersQuery query, CancellationToken ct = default)
    {
        var users = await _userRepository.GetAllAsync(query.Skip, query.Take, ct);
        return users.Select(x => new GetUsersResult
        {
            Id = x.Id,
            Email = x.Email,
            FullName = x.FullName,
            CreatedAt = x.CreatedAt,
        });
    }
}