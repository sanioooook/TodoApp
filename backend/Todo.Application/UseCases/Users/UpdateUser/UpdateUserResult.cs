namespace Todo.Application.UseCases.Users.UpdateUser;

using Enums;
using Interfaces;

public class UpdateUserResult: IResultCommand
{
    /// <inheritdoc />
    public bool Success { get; set; }

    /// <inheritdoc />
    public string Message { get; set; }

    /// <inheritdoc />
    public ResultCode CodeResult { get; set; }
}