namespace Todo.Application.Models;

public class TodoListDto
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
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the owner identifier.
    /// </summary>
    /// <value>
    /// The owner identifier.
    /// </value>
    public Guid OwnerId { get; set; }

    /// <summary>
    /// Gets or sets the created at.
    /// </summary>
    /// <value>
    /// The created at.
    /// </value>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the updated at.
    /// </summary>
    /// <value>
    /// The updated at.
    /// </value>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the shared with users.
    /// </summary>
    /// <value>
    /// The shared with users.
    /// </value>
    public IEnumerable<TodoListShareDto> SharedWithUsers { get; set; }
}