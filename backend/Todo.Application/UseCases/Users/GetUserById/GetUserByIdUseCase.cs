namespace Todo.Application.UseCases.Users.GetUserById;

using Enums;
using Interfaces;
using Models;

public class GetUserByIdUseCase : IGetUserByIdUseCase
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    /// <inheritdoc />
    public async Task<GetUserByIdResult> ExecuteAsync(GetUserByIdQuery query, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdAsync(query.Id, ct);
        if (user is null)
            return new GetUserByIdResult { HaveResult = false, Message = "User not found", CodeResult = ResultCode.NotFound };

        return new GetUserByIdResult
        {
            User = new UserDto
            {
                Email = user.Email,
                FullName = user.FullName,
                Id = user.Id,
                CreatedAt = user.CreatedAt,
            },
            HaveResult = true,
            Message = string.Empty,
            CodeResult = ResultCode.Success,
        };
    }
}