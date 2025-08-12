namespace Todo.Application.UseCases.Users.GetUsers;

public class GetUsersQuery
{
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