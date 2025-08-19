namespace Todo.Domain.Entities;

public class TodoList
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public Guid OwnerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public User? Owner { get; set; }
    public ICollection<TodoListShare> Shares { get; set; } = new List<TodoListShare>();
}