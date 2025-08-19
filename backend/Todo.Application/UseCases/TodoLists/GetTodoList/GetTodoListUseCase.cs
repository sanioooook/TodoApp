using Todo.Application.Common;

namespace Todo.Application.UseCases.TodoLists.GetTodoList;

using FluentResults;
using Interfaces;
using Models;
using Todo.Application.Models.TodoList;

public class GetTodoListUseCase(IListRepository repository) : IGetTodoListUseCase
{
    /// <inheritdoc />
    public async Task<Result<TodoListDto>> HandleAsync(GetTodoListQuery query, CancellationToken ct)
    {
        var list = await repository.GetByIdAsync(query.ListId, ct);
        if (list is null)
            return Result.Fail(Errors.NotFound("TodoList", query.ListId));

        return Result.Ok(new TodoListDto
        {
            Id = list.Id,
            Title = list.Title,
            CreatedAt = list.CreatedAt,
            UpdatedAt = list.UpdatedAt,
            OwnerId = list.OwnerId,
            SharedWithUsers = list.Shares.Select(x => new TodoListShareDto { UserId = x.UserId, UserFullName = x.User.FullName }),
        });
    }
}