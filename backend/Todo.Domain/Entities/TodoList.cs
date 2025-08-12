﻿namespace Todo.Domain.Entities;

public class TodoList
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public Guid OwnerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<TodoListShare> Shares { get; set; } = new();
}