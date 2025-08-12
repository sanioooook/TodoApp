using System.ComponentModel.DataAnnotations;

namespace Todo.Application.UseCases.TodoLists.GetTodoList;

public class GetTodoListQuery
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    /// <value>
    /// The user identifier.
    /// </value>
    [Required]
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the list identifier.
    /// </summary>
    /// <value>
    /// The list identifier.
    /// </value>
    [Required]
    public Guid ListId { get; set; }
}