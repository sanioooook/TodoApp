namespace Todo.Application.UseCases.Users.GetUserById;

using System.ComponentModel.DataAnnotations;

public class GetUserByIdQuery
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    [Required]
    public Guid Id { get; set; }
}