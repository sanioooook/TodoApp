namespace Todo.Application.UseCases.Users.DeleteUser;

using Enums;
using Interfaces;

public class DeleteUserResult : IResultCommand
{
    /// <inheritdoc />
    public bool Success { get; set; }

    /// <inheritdoc />
    public string Message { get; set; }

    /// <inheritdoc />
    public ResultCode CodeResult { get; set; }
}