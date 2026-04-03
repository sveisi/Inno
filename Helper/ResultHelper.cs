using Inno.Services;
using Microsoft.AspNetCore.Mvc;

namespace Inno.Helper
{
    public class Result
    {
        public bool Success { get; }
        public string Error { get; }
        public bool IsFailure => !Success;

        protected Result(bool success, string error = null)
        {
            Success = success;
            Error = error;
        }

        public static Result Ok() => new(true);
        public static Result Failure(string error) => new(false, error);

        public static implicit operator Result(string error) => Failure(error);
    }

    public class Result<T> : Result
    {
        public T Data { get; }

        private Result(bool success, T data, string error) : base(success, error)
        {
            Data = data;
        }

        public static Result<T> Ok(T value) => new(true, value, null);
        public static new Result<T> Failure(string error) => new(false, default, error);

        // تبدیل ضمنی (Implicit) برای کدنویسی بسیار سریع‌تر
        // این اجازه می‌دهد مستقیماً بنویسید: return "Error Message"; یا return myData;
        public static implicit operator Result<T>(T value) => Ok(value);
        public static implicit operator Result<T>(string error) => Failure(error);
    }

    public static class ResultHelper
    {
        public static IActionResult ToActionResult(this Result result)
        {
            if (result.Success)
                return new OkObjectResult(new { success = true });

            return new OkObjectResult(new { success = false, error = result.Error });
        }

        public static IActionResult ToActionResult<T>(this Result<T> result)
        {
            if (result.Success)
                return new OkObjectResult(new { success = true, data = result.Data });

            return new OkObjectResult(new { success = false, error = result.Error });
        }
    }
}
