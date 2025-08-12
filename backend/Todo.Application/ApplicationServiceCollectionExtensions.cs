namespace Todo.Application;

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using UseCases.TodoLists.CreateTodoList;
using UseCases.TodoLists.DeleteTodoList;
using UseCases.TodoLists.GetTodoList;
using UseCases.TodoLists.GetUserTodoLists;
using UseCases.TodoLists.ShareTodoList;
using UseCases.TodoLists.UnshareTodoList;
using UseCases.TodoLists.UpdateTodoList;
using UseCases.Users.CreateUser;
using UseCases.Users.DeleteUser;
using UseCases.Users.GetUserById;
using UseCases.Users.GetUsers;
using UseCases.Users.UpdateUser;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DeleteUserValidator).Assembly);

        // todoList use cases
        services.AddScoped<ICreateTodoListUseCase, CreateTodoListUseCase>();
        services.AddScoped<IUpdateTodoListUseCase, UpdateTodoListUseCase>();
        services.AddScoped<IDeleteTodoListUseCase, DeleteTodoListUseCase>();
        services.AddScoped<IGetUserTodoListsUseCase, GetUserTodoListsUseCase>();
        services.AddScoped<IGetTodoListUseCase, GetTodoListUseCase>();
        services.AddScoped<IShareTodoListUseCase, ShareTodoListUseCase>();
        services.AddScoped<IUnshareTodoListUseCase, UnshareTodoListUseCase>();

        // User use cases
        services.AddScoped<ICreateUserUseCase, CreateUserUseCase>();
        services.AddScoped<IUpdateUserUseCase, UpdateUserUseCase>();
        services.AddScoped<IDeleteUserUseCase, DeleteUserUseCase>();
        services.AddScoped<IGetUsersUseCase, GetUsersUseCase>();
        services.AddScoped<IGetUserByIdUseCase, GetUserByIdUseCase>();

        return services;
    }
}