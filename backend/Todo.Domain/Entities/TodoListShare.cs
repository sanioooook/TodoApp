namespace Todo.Domain.Entities;

public class TodoListShare
{
    public Guid Id { get; set; }
    public Guid TodoListId { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
}