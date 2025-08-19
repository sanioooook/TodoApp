namespace Todo.Application.UseCases.TodoLists.CreateTodoList;

public class CreateTodoListCommand
{
    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    /// <value>
    /// The title.
    /// </value>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the owner identifier.
    /// </summary>
    /// <value>
    /// The owner identifier.
    /// </value>
    public Guid OwnerId { get; set; }
}