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
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    /// <value>
    /// The title.
    /// </value>
    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the owner identifier.
    /// </summary>
    /// <value>
    /// The owner identifier.
    /// </value>
    [Required]
    public Guid CurrentUserId { get; set; }
}