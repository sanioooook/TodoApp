namespace Todo.Application.Models.TodoList;

public record TodoListShareDto
{
    public Guid UserId { get; set; }
    public string UserFullName { get; set; }
}