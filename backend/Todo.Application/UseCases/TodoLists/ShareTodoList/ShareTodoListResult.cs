namespace Todo.Application.UseCases.TodoLists.ShareTodoList;

using Enums;
using Interfaces;

public class ShareTodoListResult : IResultCommand
{
    /// <inheritdoc />
    public bool Success { get; set; }

    /// <inheritdoc />
    public string Message { get; set; }

    /// <inheritdoc />
    public ResultCode CodeResult { get; set; }
}