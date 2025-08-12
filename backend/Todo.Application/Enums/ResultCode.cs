namespace Todo.Application.Enums;

public enum ResultCode
{
    Success = 200,
    BadRequest = 400,
    Forbidden = 403,
    NotFound = 404,
    Conflict = 409,
    ServerError = 500
}