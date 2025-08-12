namespace Todo.Application.UseCases.TodoLists.DeleteTodoList;

using System.ComponentModel.DataAnnotations;

public class DeleteTodoListCommand
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
    /// Gets or sets the current user identifier.
    /// </summary>
    /// <value>
    /// The current user identifier.
    /// </value>
    [Required]
    public Guid CurrentUserId { get; set; }
}