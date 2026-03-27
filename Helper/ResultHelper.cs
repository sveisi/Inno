using Inno.Services;
using Microsoft.AspNetCore.Mvc;

namespace Inno.Helper
{
    public class Result<T>
    {
        public bool Success { get; init; }
        public T? Data { get; init; }
        public string Error { get; init; }

        public static Result<T> Ok(T data) => new() { Success = true, Data = data };
        public static Result<T> Failure(string error) => new() { Success = false, Error = error };
    }

    public static class ResultHelper
    {
        public static IActionResult ToActionResult<T>(this Result<T> result)
        {
            if (result.Success)
                return new OkObjectResult(new { success = true, data = result.Data });

            return new OkObjectResult(new { success = false, error = result.Error });
        }
    }
}
