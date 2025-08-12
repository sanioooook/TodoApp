namespace Todo.Application.UseCases.TodoLists.CreateTodoList;

using Enums;
using Interfaces;
using Models;

public class CreateTodoListResult : IResultCommand
{
    /// <summary>
    /// Gets or sets the todoList dto.
    /// </summary>
    /// <value>
    /// The todoList dto.
    /// </value>
    public TodoListDto TodoListDto { get; set; }

    /// <inheritdoc />
    public bool Success { get; set; }

    /// <inheritdoc />
    public string Message { get; set; }

    /// <inheritdoc />
    public ResultCode CodeResult { get; set; }
}