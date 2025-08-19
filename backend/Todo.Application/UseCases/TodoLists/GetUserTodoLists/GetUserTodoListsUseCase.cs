namespace Todo.Application.UseCases.TodoLists.GetUserTodoLists;

using FluentResults;
using Interfaces;
using Models.TodoList;

public class GetUserTodoListsUseCase(IListRepository repository) : IGetUserTodoListsUseCase
{
    /// <inheritdoc />
    public async Task<Result<IEnumerable<TodoListDto>>> HandleAsync(GetUserTodoListsQuery query, CancellationToken ct)
    {
        var lists = await repository.GetForUserAsync(query.UserId, query.Skip, query.Take, ct);
        return Result.Ok(lists.Select(l => new TodoListDto
        {
            Id = l.Id,
            Title = l.Title,
            CreatedAt = l.CreatedAt,
            UpdatedAt = l.UpdatedAt,
            OwnerId = l.OwnerId,
            SharedWithUsers = l.Shares
                .Select(x => new TodoListShareDto
                {
                    UserId = x.UserId,
                    UserFullName = x.User.FullName
                }),
        }));
    }
}