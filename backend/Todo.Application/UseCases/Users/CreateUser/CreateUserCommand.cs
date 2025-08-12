namespace Todo.Application.UseCases.Users.CreateUser;

using System.ComponentModel.DataAnnotations;

public class CreateUserCommand
{
    /// <summary>
    /// Gets or sets the email.
    /// </summary>
    /// <value>
    /// The email.
    /// </value>
    [Required]
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the full name.
    /// </summary>
    /// <value>
    /// The full name.
    /// </value>
    [Required]
    public string FullName { get; set; }
}