using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Todo.Application.Enums;
using Todo.Application.Models;
using Todo.Application.UseCases.TodoLists.CreateTodoList;
using Todo.Application.UseCases.TodoLists.DeleteTodoList;
using Todo.Application.UseCases.TodoLists.GetTodoList;
using Todo.Application.UseCases.TodoLists.GetUserTodoLists;
using Todo.Application.UseCases.TodoLists.ShareTodoList;
using Todo.Application.UseCases.TodoLists.UnshareTodoList;
using Todo.Application.UseCases.TodoLists.UpdateTodoList;

namespace Todo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ListsController(ILogger<ListsController> _logger) : ControllerBase
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
        if (!Guid.TryParse(Request.Headers["X-User-Id"], out var userId))
            return BadRequest("X-User-Id header is missing or invalid.");

        var query = new GetUserTodoListsQuery
        {
            UserId = userId,
            Skip = skip,
            Take = take
        };

        var badRequest = await this.ValidateAndReturnIfInvalid(validator, query);
        if (badRequest != null)
            return badRequest;

        var result = await getUserLists.HandleAsync(query, HttpContext.RequestAborted);
        return Ok(result);
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
        [FromServices] IValidator<ShareTodoListCommand> validator
        )
    {
        if (!Guid.TryParse(Request.Headers["X-User-Id"], out var userId))
            return BadRequest("X-User-Id header is missing or invalid.");

        var command = new ShareTodoListCommand
        {
            ListId = id,
            CurrentUserId = userId,
            TargetUserId = targetUserId
        };

        var badRequest = await this.ValidateAndReturnIfInvalid(validator, command);
        if (badRequest != null)
            return badRequest;

        _logger.LogInformation("Try sharing TodoList {ListId} with user {UserId}", command.ListId, command.TargetUserId);

        var result = await share.HandleAsync(command, HttpContext.RequestAborted);

        if (result.CodeResult != ResultCode.Success)
        {
            _logger.LogInformation("Failure sharing TodoList {ListId} with user {UserId}. Error message {Message}", command.ListId, command.TargetUserId, result.Message);
            return this.HandleNonSuccess(result);
        }

        _logger.LogInformation("Success sharing TodoList {ListId} with user {UserId}", command.ListId, command.TargetUserId);
        return NoContent();
    }

    /// <summary>Unshares the list.</summary>
    /// <param name="id">The TodoList identifier.</param>
    /// <param name="targetUserId">The target user identifier.</param>
    /// <param name="unshare">The Use case.</param>
    /// <param name="validator">The validator.</param>
    [HttpPost("{id:guid}/unshare")]
    public async Task<IActionResult> UnshareList(
        Guid id,
        [FromBody] Guid targetUserId,
        [FromServices] IUnshareTodoListUseCase unshare,
        [FromServices] IValidator<UnshareTodoListCommand> validator)
    {
        if (!Guid.TryParse(Request.Headers["X-User-Id"], out var userId))
            return BadRequest("X-User-Id header is missing or invalid.");

        var command = new UnshareTodoListCommand
        {
            ListId = id,
            CurrentUserId = userId,
            TargetUserId = targetUserId
        };

        var badRequest = await this.ValidateAndReturnIfInvalid(validator, command);
        if (badRequest != null)
            return badRequest;

        _logger.LogInformation("Try unsharing TodoList {ListId} with user {UserId}", command.ListId, command.TargetUserId);
        var result = await unshare.HandleAsync(command, HttpContext.RequestAborted);

        if (result.CodeResult != ResultCode.Success)
        {
            _logger.LogInformation("Failure unsharing TodoList {ListId} with user {UserId}. Error message {Message}", command.ListId, command.TargetUserId, result.Message);
            return this.HandleNonSuccess(result);
        }
        _logger.LogInformation("Success unsharing TodoList {ListId} with user {UserId}", command.ListId, command.TargetUserId);
        return NoContent();
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
        if (!Guid.TryParse(Request.Headers["X-User-Id"], out var userId))
            return BadRequest("X-User-Id header is missing or invalid.");
        var query = new GetTodoListQuery { UserId = userId, ListId = id };

        var badRequest = await this.ValidateAndReturnIfInvalid(validator, query);
        if (badRequest != null)
            return badRequest;

        var result = await getList.HandleAsync(query, HttpContext.RequestAborted);

        if (result.CodeResult != ResultCode.Success)
            return this.HandleNonSuccess(result);
        return Ok(result.TodoListDto);
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
        if (!Guid.TryParse(Request.Headers["X-User-Id"], out var userId))
            return BadRequest("X-User-Id header is missing or invalid.");

        var command = new CreateTodoListCommand { OwnerId = userId, Title = todoListCreateDto.Title };

        var badRequest = await this.ValidateAndReturnIfInvalid(validator, command);
        if (badRequest != null)
            return badRequest;

        var result = await create.HandleAsync(command, HttpContext.RequestAborted);

        if (result.CodeResult != ResultCode.Success)
            return this.HandleNonSuccess(result);

        return CreatedAtAction(nameof(Create), new { id = result.TodoListDto.Id }, result.TodoListDto);
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
        if (!Guid.TryParse(Request.Headers["X-User-Id"], out var userId))
            return BadRequest("X-User-Id header is missing or invalid.");
        var command = new UpdateTodoListCommand { Id = updateDto.Id, CurrentUserId = userId, Title = updateDto.Title };

        var badRequest = await this.ValidateAndReturnIfInvalid(validator, command);
        if (badRequest != null)
            return badRequest;

        var result = await update.HandleAsync(command, HttpContext.RequestAborted);

        if (result.CodeResult != ResultCode.Success)
            return this.HandleNonSuccess(result);
        return NoContent();
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
        if (!Guid.TryParse(Request.Headers["X-User-Id"], out var userId))
            return BadRequest("X-User-Id header is missing or invalid.");

        var command = new DeleteTodoListCommand
        {
            Id = id,
            CurrentUserId = userId
        };

        var badRequest = await this.ValidateAndReturnIfInvalid(validator, command);
        if (badRequest != null)
            return badRequest;

        var result = await delete.HandleAsync(command, HttpContext.RequestAborted);

        if (result.CodeResult != ResultCode.Success)
            return this.HandleNonSuccess(result);
        return NoContent();
    }
}