namespace Todo.Application.UseCases.TodoLists.UpdateTodoList;

using System.ComponentModel.DataAnnotations;

public class UpdateTodoListCommand
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    /// <value>
    /// The title.
    /// </value>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the owner identifier.
    /// </summary>
    /// <value>
    /// The owner identifier.
    /// </value>
    public Guid CurrentUserId { get; set; }
}