namespace Todo.Application.UseCases.TodoLists.CreateTodoList;

using Domain.Entities;
using Enums;
using Interfaces;
using Models;

public class CreateTodoListUseCase : ICreateTodoListUseCase
{
    private readonly IListRepository _repository;
    private readonly IUserRepository _userRepository;

    public CreateTodoListUseCase(IListRepository repository, IUserRepository userRepository)
    {
        _repository = repository;
        _userRepository = userRepository;
    }

    /// <inheritdoc />
    public async Task<CreateTodoListResult> HandleAsync(CreateTodoListCommand command, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(command.OwnerId, ct);

        if (user is null)
            return new CreateTodoListResult { Success = false, Message = "OwnerId is wrong", CodeResult = ResultCode.BadRequest };

        if (command.Title.Length is < 1 or > 255)
            return new CreateTodoListResult { Success = false, Message = "Title length need be more 1 symbol and less 255", CodeResult = ResultCode.BadRequest };

        var now = DateTime.UtcNow;

        var entity = new TodoList
        {
            Id = Guid.NewGuid(),
            Title = command.Title,
            OwnerId = command.OwnerId,
            CreatedAt = now,
            UpdatedAt = now
        };

        var created = await _repository.AddAsync(entity, ct);

        if (!created)
            return new CreateTodoListResult { Success = false, Message = "Error while creating TodoList", CodeResult = ResultCode.ServerError };

        return new CreateTodoListResult
        {
            TodoListDto = new TodoListDto
            {
                Title = entity.Title,
                Id = entity.Id,
                OwnerId = entity.OwnerId,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                SharedWithUsers = [],
            },
            Success = true,
            Message = "Successfully created TodoList",
            CodeResult = ResultCode.Success
        };
    }
}