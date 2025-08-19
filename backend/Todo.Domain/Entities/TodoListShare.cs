namespace Todo.Domain.Entities;

public class TodoListShare
{
    public Guid TodoListId { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public TodoList? TodoList { get; set; }
    public User? User { get; set; }
}