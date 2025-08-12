namespace Todo.Application.UseCases.Users.DeleteUser;

using System.ComponentModel.DataAnnotations;

public class DeleteUserCommand
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