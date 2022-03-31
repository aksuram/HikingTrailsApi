using System;
using System.Collections.Generic;
using System.Linq;

namespace HikingTrailsApi.Application.Models
{
    public class Result<T> where T : class
    {
        public bool Succeeded { get; set; }
        public FieldError[] Errors { get; set; }
        public T Value { get; set; }

        private Result(bool succeeded, IEnumerable<FieldError> errors, T value)
        {
            Succeeded = succeeded;
            Errors = errors.ToArray();
            Value = value;
        }

        public static Result<T> Success(T value)
        {
            return new Result<T>(true, Array.Empty<FieldError>(), value);
        }

        public static Result<T> Failure(IEnumerable<FieldError> errors)
        {
            return new Result<T>(false, errors, null);
        }

        public static Result<T> Failure()
        {
            return Failure(new FieldError[] { new FieldError("Unknown", "Unknwon error occured") });
        }

        public static Result<T> Failure(string field, string error)
        {
            return Failure(new FieldError[] { new FieldError(field, error) });
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

        public static Result Failure()
        {
            return Failure(new FieldError[] { new FieldError("Unknown", "Unknown error occured") });
        }

        public static Result Failure(string field, string error)
        {
            return Failure(new FieldError[] { new FieldError(field, error) });
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
