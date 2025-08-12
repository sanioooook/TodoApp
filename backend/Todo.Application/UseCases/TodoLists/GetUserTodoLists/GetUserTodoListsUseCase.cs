namespace Todo.Application.UseCases.TodoLists.GetUserTodoLists;

using Interfaces;

public class GetUserTodoListsUseCase : IGetUserTodoListsUseCase
{
    private readonly IListRepository _repository;

    public GetUserTodoListsUseCase(IListRepository repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<GetUserTodoListsResult>> HandleAsync(GetUserTodoListsQuery query, CancellationToken ct)
    {
        var lists = await _repository.GetForUserAsync(query.UserId, query.Skip, query.Take, ct);
        return lists.Select(l => new GetUserTodoListsResult
        {
            Id = l.Id,
            Title = l.Title,
            CreatedAt = l.CreatedAt,
            UpdatedAt = l.UpdatedAt,
            OwnerId = l.OwnerId,
            SharedWithUsers = [],
        });
    }
}