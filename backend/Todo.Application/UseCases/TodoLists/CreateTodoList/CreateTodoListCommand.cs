namespace Todo.Application.UseCases.TodoLists.CreateTodoList;

using System.ComponentModel.DataAnnotations;

public class CreateTodoListCommand
{
    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    /// <value>
    /// The title.
    /// </value>
    [Required]
    [MaxLength(255)]
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the owner identifier.
    /// </summary>
    /// <value>
    /// The owner identifier.
    /// </value>
    [Required]
    public Guid OwnerId { get; set; }
}