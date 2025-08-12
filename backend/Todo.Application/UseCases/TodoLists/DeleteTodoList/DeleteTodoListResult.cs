namespace Todo.Application.UseCases.TodoLists.DeleteTodoList;

using Enums;
using Interfaces;

public class DeleteTodoListResult : IResultCommand
{
    /// <inheritdoc />
    public bool Success { get; set; }

    /// <inheritdoc />
    public string Message { get; set; }

    /// <inheritdoc />
    public ResultCode CodeResult { get; set; }
}