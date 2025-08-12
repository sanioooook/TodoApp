namespace Todo.Application.UseCases.TodoLists.UnshareTodoList;

using System.ComponentModel.DataAnnotations;

public class UnshareTodoListCommand
{
    /// <summary>
    /// Gets or sets the list identifier.
    /// </summary>
    /// <value>
    /// The list identifier.
    /// </value>
    [Required]
    public Guid ListId { get; set; }

    /// <summary>
    /// Gets or sets the owner identifier.
    /// </summary>
    /// <value>
    /// The owner identifier.
    /// </value>
    [Required]
    public Guid CurrentUserId { get; set; }

    /// <summary>
    /// Gets or sets the target user identifier.
    /// </summary>
    /// <value>
    /// The target user identifier.
    /// </value>
    [Required]
    public Guid TargetUserId { get; set; }
}