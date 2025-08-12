namespace Todo.Application.UseCases.TodoLists.GetTodoList;

using Enums;
using Interfaces;
using Models;

public class GetTodoListResult : IResultQuery
{
    /// <summary>
    /// Gets or sets the todoList dto.
    /// </summary>
    /// <value>
    /// The todoList dto.
    /// </value>
    public TodoListDto TodoListDto { get; set; }

    /// <inheritdoc />
    public bool HaveResult { get; set; }

    /// <inheritdoc />
    public string Message { get; set; }

    /// <inheritdoc />
    public ResultCode CodeResult { get; set; }
}