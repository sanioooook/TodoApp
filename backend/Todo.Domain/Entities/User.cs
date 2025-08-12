namespace Todo.Domain.Entities;

using System.ComponentModel.DataAnnotations.Schema;

public class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;

    [Column("full_name")]
    public string FullName { get; set; } = null!;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}