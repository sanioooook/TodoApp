namespace Todo.Application.UseCases.Users.GetUserById;

using Enums;
using Interfaces;
using Models;

public class GetUserByIdResult: IResultQuery
{
    /// <summary>
    /// Gets or sets the user.
    /// </summary>
    /// <value>
    /// The user.
    /// </value>
    public UserDto User { get; set; }

    /// <inheritdoc />
    public bool HaveResult { get; set; }

    /// <inheritdoc />
    public string Message { get; set; }

    /// <inheritdoc />
    public ResultCode CodeResult { get; set; }
}