namespace Todo.Application.Interfaces;

public interface IResultQuery : IResult
{
    /// <summary>
    /// Gets or sets a value indicating whether [have result].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [have result]; otherwise, <c>false</c>.
    /// </value>
    bool HaveResult { get; set; }
}