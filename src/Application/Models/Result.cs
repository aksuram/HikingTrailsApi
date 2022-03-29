using System;
using System.Collections.Generic;
using System.Linq;

namespace HikingTrailsApi.Application.Models
{
    public class Result<T> where T : class
    {
        T Value { get; set; }
        public bool Succeeded { get; set; }
        public FieldError[] Errors { get; set; }

        private Result(T value, bool succeeded, IEnumerable<FieldError> errors)
        {
            Value = value;
            Succeeded = succeeded;
            Errors = errors.ToArray();
        }

        public static Result<T> Success(T value)
        {
            return new Result<T>(value, true, Array.Empty<FieldError>());
        }

        public static Result<T> Failure(IEnumerable<FieldError> errors)
        {
            return new Result<T>(null, false, errors);
        }
    }

    public class Result
    {
        public bool Succeeded { get; set; }
        public FieldError[] Errors { get; set; }

        private Result(bool succeeded, IEnumerable<FieldError> errors)
        {
            Succeeded = succeeded;
            Errors = errors.ToArray();
        }

        public static Result Success()
        {
            return new Result(true, Array.Empty<FieldError>());
        }

        public static Result Failure(IEnumerable<FieldError> errors)
        {
            return new Result(false, errors);
        }
    }

    public class FieldError
    {
        public FieldError(string field, string error)
        {
            Field = field;
            Error = error;
        }

        public string Field { get; set; }
        public string Error { get; set; }
    }
}
