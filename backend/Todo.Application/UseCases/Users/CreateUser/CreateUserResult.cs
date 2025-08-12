namespace Todo.Application.UseCases.Users.CreateUser;

using Enums;
using Interfaces;

public class CreateUserResult : IResultCommand
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    public Guid Id { get; set; }

    /// <inheritdoc />
    public bool Success { get; set; }

    /// <inheritdoc />
    public string Message { get; set; }

    /// <inheritdoc />
    public ResultCode CodeResult { get; set; }
}