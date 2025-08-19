namespace Todo.Api.Controllers;

using Application.Models.TodoList;
using Application.UseCases.TodoLists.CreateTodoList;
using Application.UseCases.TodoLists.DeleteTodoList;
using Application.UseCases.TodoLists.GetTodoList;
using Application.UseCases.TodoLists.GetUserTodoLists;
using Application.UseCases.TodoLists.ShareTodoList;
using Application.UseCases.TodoLists.UnshareTodoList;
using Application.UseCases.TodoLists.UpdateTodoList;
using Common;
using Filters;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[TypeFilter(typeof(CurrentUserFilter))]
public class ListsController(ILogger<ListsController> logger) : ControllerBase
{
    /// <summary>
    /// Gets the TodoLists by user.
    /// </summary>
    /// <param name="getUserLists">The Use case.</param>
    /// <param name="validator">The validator.</param>
    /// <param name="skip">The skip.</param>
    /// <param name="take">The take.</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoListDto>>> GetLists(
        [FromServices] IGetUserTodoListsUseCase getUserLists,
        [FromServices] IValidator<GetUserTodoListsQuery> validator,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20)
    {
        var query = new GetUserTodoListsQuery
        {
            UserId = HttpContext.GetCurrentUserId(),
            Skip = skip,
            Take = take
        };

        var validationResult = await validator.ValidateWithResultAsync(query, HttpContext.RequestAborted);
        if (validationResult.IsFailed)
            return (ActionResult)validationResult.ToActionResult();

        var result = await getUserLists.HandleAsync(query, HttpContext.RequestAborted);
        
        return (ActionResult)result.ToActionResult();
    }

    /// <summary>Shares the todoList.</summary>
    /// <param name="id">The TodoList identifier.</param>
    /// <param name="targetUserId">The target user identifier.</param>
    /// <param name="share">The Use case.</param>
    /// <param name="validator">The validator.</param>
    [HttpPost("{id:guid}/share")]
    public async Task<IActionResult> ShareList(Guid id,
        [FromBody] Guid targetUserId,
        [FromServices] IShareTodoListUseCase share,
        [FromServices] IValidator<ShareTodoListCommand> validator)
    {
        var command = new ShareTodoListCommand
        {
            ListId = id,
            CurrentUserId = HttpContext.GetCurrentUserId(),
            TargetUserId = targetUserId
        };

        var validationResult = await validator.ValidateWithResultAsync(command, HttpContext.RequestAborted);
        if (validationResult.IsFailed)
            return validationResult.ToActionResult();

        logger.LogInformation("Try sharing TodoList {ListId} with user {UserId}", command.ListId, command.TargetUserId);

        var result = await share.HandleAsync(command, HttpContext.RequestAborted);

        return result.ToActionResult();
    }

    /// <summary>Unshares the list.</summary>
    /// <param name="id">The TodoList identifier.</param>
    /// <param name="targetUserId">The target user identifier.</param>
    /// <param name="unshare">The Use case.</param>
    /// <param name="validator">The validator.</param>
    [HttpPost("{id:guid}/unshare")]
    public async Task<IActionResult> UnShareList(
        Guid id,
        [FromBody] Guid targetUserId,
        [FromServices] IUnshareTodoListUseCase unshare,
        [FromServices] IValidator<UnshareTodoListCommand> validator)
    {
        var command = new UnshareTodoListCommand
        {
            ListId = id,
            CurrentUserId = HttpContext.GetCurrentUserId(),
            TargetUserId = targetUserId
        };

        var validationResult = await validator.ValidateWithResultAsync(command, HttpContext.RequestAborted);
        if (validationResult.IsFailed)
            return validationResult.ToActionResult();

        logger.LogInformation("Try unsharing TodoList {ListId} with user {UserId}", command.ListId, command.TargetUserId);
        var result = await unshare.HandleAsync(command, HttpContext.RequestAborted);

        return result.ToActionResult();
    }

    /// <summary>Gets the list by identifier.</summary>
    /// <param name="id">The identifier list.</param>
    /// <param name="getList">The Use case.</param>
    /// <param name="validator">The validator.</param>
    /// <returns>TodoList object</returns>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetListById(
        Guid id,
        [FromServices] IGetTodoListUseCase getList,
        [FromServices] IValidator<GetTodoListQuery> validator)
    {
        var query = new GetTodoListQuery { UserId = HttpContext.GetCurrentUserId(), ListId = id };

        var validationResult = await validator.ValidateWithResultAsync(query, HttpContext.RequestAborted);
        if (validationResult.IsFailed)
            return validationResult.ToActionResult();

        var result = await getList.HandleAsync(query, HttpContext.RequestAborted);

        return result.ToActionResult();
    }

    /// <summary>Creates the TodoList.</summary>
    /// <param name="todoListCreateDto">Title of TodoList.</param>
    /// <param name="create">The Use case.</param>
    /// <param name="validator">The validator.</param>
    /// <returns>Created TodoList object</returns>
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] TodoListCreateDto todoListCreateDto,
        [FromServices] ICreateTodoListUseCase create,
        [FromServices] IValidator<CreateTodoListCommand> validator)
    {
        var command = new CreateTodoListCommand { OwnerId = HttpContext.GetCurrentUserId(), Title = todoListCreateDto.Title };

        var validationResult = await validator.ValidateWithResultAsync(command, HttpContext.RequestAborted);
        if (validationResult.IsFailed)
            return validationResult.ToActionResult();

        var result = await create.HandleAsync(command, HttpContext.RequestAborted);

        return result.ToActionResult();
    }

    /// <summary>Updates the specified TodoList by identifier.</summary>
    /// <param name="updateDto">The update object</param>
    /// <param name="update">The Use case.</param>
    /// <param name="validator">The validator.</param>
    [HttpPut]
    public async Task<IActionResult> Update(
        [FromBody] TodoListUpdateDto updateDto,
        [FromServices] IUpdateTodoListUseCase update,
        [FromServices] IValidator<UpdateTodoListCommand> validator)
    {
        var command = new UpdateTodoListCommand
        {
            Id = updateDto.Id,
            CurrentUserId = HttpContext.GetCurrentUserId(),
            Title = updateDto.Title,
        };

        var validationResult = await validator.ValidateWithResultAsync(command, HttpContext.RequestAborted);
        if (validationResult.IsFailed)
            return validationResult.ToActionResult();

        var result = await update.HandleAsync(command, HttpContext.RequestAborted);

        return result.ToActionResult();
    }

    /// <summary>Deletes the TodoList by identifier.</summary>
    /// <param name="id">The TodoList identifier.</param>
    /// <param name="delete">The Use case.</param>
    /// <param name="validator">The validator.</param>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        [FromServices] IDeleteTodoListUseCase delete,
        [FromServices] IValidator<DeleteTodoListCommand> validator)
    {
        var command = new DeleteTodoListCommand
        {
            Id = id,
            CurrentUserId = HttpContext.GetCurrentUserId()
        };

        var validationResult = await validator.ValidateWithResultAsync(command, HttpContext.RequestAborted);
        if (validationResult.IsFailed)
            return validationResult.ToActionResult();

        var result = await delete.HandleAsync(command, HttpContext.RequestAborted);
        
        return result.ToActionResult();
    }
}