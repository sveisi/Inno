using Inno.Services;
using Microsoft.AspNetCore.Mvc;

namespace Inno.Helper
{
    public class Result<T>
    {
        public bool IsSuccess { get; init; }
        public T? Data { get; init; }
        public string Error { get; init; }

        public static Result<T> Success(T data) => new() { IsSuccess = true, Data = data };
        public static Result<T> Failure(string error) => new() { IsSuccess = false, Error = error };
    }

    public static class ResultHelper
    {
        public static IActionResult ToActionResult<T>(this Result<T> result)
        {
            if (result.IsSuccess)
                return new OkObjectResult(new { success = true, data = result.Data });

            return new OkObjectResult(new { success = false, error = result.Error });
        }
    }
}
