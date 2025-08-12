namespace Todo.Application.UseCases.TodoLists.ShareTodoList;

using System.ComponentModel.DataAnnotations;

public class ShareTodoListCommand
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
    /// Gets or sets the current user identifier.
    /// </summary>
    /// <value>
    /// The current user identifier.
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