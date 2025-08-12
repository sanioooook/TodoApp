namespace Todo.Application.Interfaces;

public interface IResultCommand : IResult
{
    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="IResultCommand"/> is success.
    /// </summary>
    /// <value>
    ///   <c>true</c> if success; otherwise, <c>false</c>.
    /// </value>
    bool Success { get; set; }
}