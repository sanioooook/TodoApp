namespace Todo.Application.UseCases.TodoLists.CreateTodoList;

using Common;
using Domain.Entities;
using FluentResults;
using Interfaces;
using Models;

public class CreateTodoListUseCase(IListRepository repository, IUserRepository userRepository) : ICreateTodoListUseCase
{
    /// <inheritdoc />
    public async Task<Result<TodoListDto>> HandleAsync(CreateTodoListCommand command, CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(command.OwnerId, ct);

        if (user is null)
            return Result.Fail<TodoListDto>(Errors.Validation("OwnerId is wrong"));

        var entity = new TodoList
        {
            Title = command.Title,
            OwnerId = command.OwnerId,
        };

        await repository.AddAsync(entity, ct);

        return Result.Ok(new TodoListDto
        {
            Title = entity.Title,
            Id = entity.Id,
            OwnerId = entity.OwnerId,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            SharedWithUsers = [],
        });
    }
}