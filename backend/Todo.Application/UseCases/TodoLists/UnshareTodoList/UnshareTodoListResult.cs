namespace Todo.Application.UseCases.TodoLists.UnshareTodoList;

using Enums;
using Interfaces;

public class UnshareTodoListResult : IResultCommand
{
    /// <inheritdoc />
    public bool Success { get; set; }

    /// <inheritdoc />
    public string Message { get; set; }

    /// <inheritdoc />
    public ResultCode CodeResult { get; set; }
}