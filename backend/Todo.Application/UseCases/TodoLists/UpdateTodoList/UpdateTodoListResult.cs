namespace Todo.Application.UseCases.TodoLists.UpdateTodoList;

using Enums;
using Interfaces;

public class UpdateTodoListResult : IResultCommand
{
    /// <inheritdoc />
    public bool Success { get; set; }

    /// <inheritdoc />
    public string Message { get; set; }

    /// <inheritdoc />
    public ResultCode CodeResult { get; set; }
}