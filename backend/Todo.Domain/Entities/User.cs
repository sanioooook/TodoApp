namespace Todo.Domain.Entities;

using System.ComponentModel.DataAnnotations.Schema;

public class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public ICollection<TodoList> TodoLists { get; set; } = new List<TodoList>();
    public ICollection<TodoListShare> SharedLists { get; set; } = new List<TodoListShare>();
}