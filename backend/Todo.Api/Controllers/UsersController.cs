using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Todo.Application.Enums;
using Todo.Application.Models;
using Todo.Application.UseCases.Users.CreateUser;
using Todo.Application.UseCases.Users.DeleteUser;
using Todo.Application.UseCases.Users.GetUserById;
using Todo.Application.UseCases.Users.GetUsers;
using Todo.Application.UseCases.Users.UpdateUser;

namespace Todo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : Controller
{
    /// <summary>
    /// Creates the user.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="createUserUseCase">The Use case.</param>
    /// <param name="validator">The validator.</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserCommand command,
        [FromServices] ICreateUserUseCase createUserUseCase,
        [FromServices] IValidator<CreateUserCommand> validator)
    {
        var badRequest = await this.ValidateAndReturnIfInvalid(validator, command);
        if (badRequest != null)
            return badRequest;

        var result = await createUserUseCase.ExecuteAsync(command, HttpContext.RequestAborted);

        if (result.CodeResult != ResultCode.Success)
            return this.HandleNonSuccess(result);
        return CreatedAtAction(nameof(Create), new { id = result.Id },
            new UserDto { Id = result.Id, Email = command.Email, FullName = command.FullName });
    }

    /// <summary>
    /// Updates the user.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="updateUserUseCase">The Use case.</param>
    /// <param name="validator">The validator.</param>
    /// <returns></returns>
    [HttpPut]
    public async Task<IActionResult> Update(
        [FromBody] UpdateUserCommand command,
        [FromServices] IUpdateUserUseCase updateUserUseCase,
        [FromServices] IValidator<UpdateUserCommand> validator)
    {
        var badRequest = await this.ValidateAndReturnIfInvalid(validator, command);
        if (badRequest != null)
            return badRequest;

        var result = await updateUserUseCase.ExecuteAsync(command, HttpContext.RequestAborted);

        if (result.CodeResult != ResultCode.Success)
            return this.HandleNonSuccess(result);
        return NoContent();
    }

    /// <summary>
    /// Deletes the user specified identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="deleteUserUseCase">The Use case.</param>
    /// <param name="validator">The validator.</param>
    /// <returns></returns>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id,
        [FromServices] IDeleteUserUseCase deleteUserUseCase,
        [FromServices] IValidator<DeleteUserCommand> validator)
    {
        var command = new DeleteUserCommand { Id = id };

        var badRequest = await this.ValidateAndReturnIfInvalid(validator, command);
        if (badRequest != null)
            return badRequest;

        var result = await deleteUserUseCase.ExecuteAsync(command, HttpContext.RequestAborted);

        if (result.CodeResult != ResultCode.Success)
            return this.HandleNonSuccess(result);
        return NoContent();
    }

    /// <summary>
    /// Gets user the by identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="getUserByIdUseCase">The Use case.</param>
    /// <param name="validator">The validator.</param>
    /// <returns></returns>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id,
        [FromServices] IGetUserByIdUseCase getUserByIdUseCase,
        [FromServices] IValidator<GetUserByIdQuery> validator)
    {
        var query = new GetUserByIdQuery { Id = id };

        var badRequest = await this.ValidateAndReturnIfInvalid(validator, query);
        if (badRequest != null)
            return badRequest;

        var result = await getUserByIdUseCase.ExecuteAsync(query, HttpContext.RequestAborted);

        if (result.CodeResult != ResultCode.Success)
            return this.HandleNonSuccess(result);
        return Ok(result.User);
    }

    /// <summary>
    /// Gets all users.
    /// </summary>
    /// <param name="getUsersUseCase">The Use case.</param>
    /// <param name="validator">The validator.</param>
    /// <param name="skip">The skip.</param>
    /// <param name="take">The take.</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetUsersResult>>> GetAll(
        [FromServices] IGetUsersUseCase getUsersUseCase,
        [FromServices] IValidator<GetUsersQuery> validator,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20)
    {
        var query = new GetUsersQuery { Skip = skip, Take = take };

        var badRequest = await this.ValidateAndReturnIfInvalid(validator, query);
        if (badRequest != null)
            return badRequest;

        var result = await getUsersUseCase.ExecuteAsync(query, HttpContext.RequestAborted);
        return Ok(result);
    }
}