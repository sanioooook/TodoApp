namespace Todo.Application.UseCases.TodoLists.DeleteTodoList;

public class DeleteTodoListCommand
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the current user identifier.
    /// </summary>
    /// <value>
    /// The current user identifier.
    /// </value>
    public Guid CurrentUserId { get; set; }
}