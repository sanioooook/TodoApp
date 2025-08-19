namespace Todo.Application.Models.TodoList;

public record TodoListCreateDto
{
    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    /// <value>
    /// The title.
    /// </value>
    public string Title { get; set; }
}