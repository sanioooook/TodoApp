namespace Todo.Application.UseCases.TodoLists.GetUserTodoLists;

using System.ComponentModel.DataAnnotations;

public class GetUserTodoListsQuery
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
    /// Gets or sets the skip.
    /// </summary>
    /// <value>
    /// The skip.
    /// </value>
    public int Skip { get; set; } = 0;

    /// <summary>
    /// Gets or sets the take.
    /// </summary>
    /// <value>
    /// The take.
    /// </value>
    public int Take { get; set; } = 20;
}