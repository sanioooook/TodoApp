namespace Todo.Api.Controllers;

using Application.Models.User;
using Application.UseCases.Users.CreateUser;
using Application.UseCases.Users.DeleteUser;
using Application.UseCases.Users.GetUserById;
using Application.UseCases.Users.GetUsers;
using Application.UseCases.Users.UpdateUser;
using Common;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UsersController : Controller
{
    /// <summary>
    /// Creates the user.
    /// </summary>
    /// <param name="createUserDto">The create user dto.</param>
    /// <param name="createUserUseCase">The Use case.</param>
    /// <param name="validator">The validator.</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserDto createUserDto,
        [FromServices] ICreateUserUseCase createUserUseCase,
        [FromServices] IValidator<CreateUserCommand> validator)
    {
        var command = new CreateUserCommand { FullName = createUserDto.FullName, Email = createUserDto.Email };
        var validationResult = await validator.ValidateWithResultAsync(command, HttpContext.RequestAborted);
        if (validationResult.IsFailed)
            return validationResult.ToActionResult();

        var result = await createUserUseCase.ExecuteAsync(command, HttpContext.RequestAborted);

        return result.ToActionResult();
    }

    /// <summary>Updates the user.</summary>
    /// <param name="updateUserDto">The update user dto.</param>
    /// <param name="updateUserUseCase">The Use case.</param>
    /// <param name="validator">The validator.</param>
    [HttpPut]
    public async Task<IActionResult> Update(
        [FromBody] UpdateUserDto updateUserDto,
        [FromServices] IUpdateUserUseCase updateUserUseCase,
        [FromServices] IValidator<UpdateUserCommand> validator)
    {
        var command = new UpdateUserCommand
            { Email = updateUserDto.Email, FullName = updateUserDto.FullName, Id = updateUserDto.Id };
        var validationResult = await validator.ValidateWithResultAsync(command, HttpContext.RequestAborted);
        if (validationResult.IsFailed)
            return validationResult.ToActionResult();

        var result = await updateUserUseCase.ExecuteAsync(command, HttpContext.RequestAborted);

        return result.ToActionResult();
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

        var validationResult = await validator.ValidateWithResultAsync(command, HttpContext.RequestAborted);
        if (validationResult.IsFailed)
            return validationResult.ToActionResult();

        var result = await deleteUserUseCase.ExecuteAsync(command, HttpContext.RequestAborted);

        return result.ToActionResult();
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

        var validationResult = await validator.ValidateWithResultAsync(query, HttpContext.RequestAborted);
        if (validationResult.IsFailed)
            return validationResult.ToActionResult();

        var result = await getUserByIdUseCase.ExecuteAsync(query, HttpContext.RequestAborted);

        return result.ToActionResult();
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
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll(
        [FromServices] IGetUsersUseCase getUsersUseCase,
        [FromServices] IValidator<GetUsersQuery> validator,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20)
    {
        var query = new GetUsersQuery { Skip = skip, Take = take };

        var validationResult = await validator.ValidateWithResultAsync(query, HttpContext.RequestAborted);
        if (validationResult.IsFailed)
            return (ActionResult)validationResult.ToActionResult();

        var result = await getUsersUseCase.ExecuteAsync(query, HttpContext.RequestAborted);
        return (ActionResult)result.ToActionResult();
    }
}