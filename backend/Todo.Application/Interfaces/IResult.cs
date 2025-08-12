namespace Todo.Application.Interfaces;

using Enums;

public interface IResult
{
    /// <summary>
    /// Gets or sets the message.
    /// </summary>
    /// <value>
    /// The message.
    /// </value>
    string Message { get; set; }

    /// <summary>
    /// Gets or sets the code result.
    /// </summary>
    /// <value>
    /// The code result.
    /// </value>
    ResultCode CodeResult { get; set; }
}