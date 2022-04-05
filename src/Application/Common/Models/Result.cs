using System.Collections.Generic;
using System.Linq;

namespace HikingTrailsApi.Application.Common.Models
{
    public class Result<T> where T : class
    {
        public ResultType Type { get; set; }
        public ResultErrors Errors { get; set; }
        public T Value { get; set; }

        private Result(ResultType type, IEnumerable<FieldError> errors, T value)
        {
            Type = type;
            Errors = new ResultErrors(errors?.ToList());
            Value = value;
        }

        public ResultErrors GetErrors()
        {
            return Errors.Errors == null ? null : Errors;
        }

        //200
        public static Result<T> Success(T value)
        {
            return new Result<T>(ResultType.Success, null, value);
        }

        //201
        public static Result<T> Created(T value)
        {
            return new Result<T>(ResultType.Created, null, value);
        }

        //204
        public static Result<T> NoContent()
        {
            return new Result<T>(ResultType.NoContent, null, null); ;
        }

        //400
        public static Result<T> BadRequest()
        {
            return new Result<T>(ResultType.BadRequest, null, null);
        }

        public static Result<T> BadRequest(IEnumerable<FieldError> errors)
        {
            return new Result<T>(ResultType.BadRequest, errors, null);
        }

        public static Result<T> BadRequest(string field, string error)
        {
            return BadRequest(new List<FieldError> { new FieldError(field, error) });
        }

        //401
        public static Result<T> Unauthorized(IEnumerable<FieldError> errors)
        {
            return new Result<T>(ResultType.Unauthorized, errors, null);
        }

        public static Result<T> Unauthorized(string field, string error)
        {
            return Unauthorized(new List<FieldError> { new FieldError(field, error) });
        }

        //403
        public static Result<T> Forbidden(IEnumerable<FieldError> errors)
        {
            return new Result<T>(ResultType.Forbidden, errors, null);
        }

        public static Result<T> Forbidden(string field, string error)
        {
            return Forbidden(new List<FieldError> { new FieldError(field, error) });
        }

        //404
        public static Result<T> NotFound(IEnumerable<FieldError> errors)
        {
            return new Result<T>(ResultType.NotFound, errors, null);
        }

        public static Result<T> NotFound(string field, string error)
        {
            return NotFound(new List<FieldError> { new FieldError(field, error) });
        }
    }

    public class Result
    {
        public ResultType Type { get; set; }
        public ResultErrors Errors { get; set; }

        private Result(ResultType type, IEnumerable<FieldError> errors)
        {
            Type = type;
            Errors = new ResultErrors(errors?.ToList());
        }

        public ResultErrors GetErrors()
        {
            return Errors.Errors == null ? null : Errors;
        }

        //200
        public static Result Success()
        {
            return new Result(ResultType.Success, null); ;
        }

        //201
        public static Result Created()
        {
            return new Result(ResultType.Created, null); ;
        }

        //204
        public static Result NoContent()
        {
            return new Result(ResultType.NoContent, null); ;
        }

        //400
        public static Result BadRequest()
        {
            return new Result(ResultType.BadRequest, null);
        }

        public static Result BadRequest(IEnumerable<FieldError> errors)
        {
            return new Result(ResultType.BadRequest, errors);
        }

        public static Result BadRequest(string field, string error)
        {
            return BadRequest(new List<FieldError> { new FieldError(field, error) });
        }

        //401
        public static Result Unauthorized(IEnumerable<FieldError> errors)
        {
            return new Result(ResultType.Unauthorized, errors);
        }

        public static Result Unauthorized(string field, string error)
        {
            return Unauthorized(new List<FieldError> { new FieldError(field, error) });
        }

        //403
        public static Result Forbidden(IEnumerable<FieldError> errors)
        {
            return new Result(ResultType.Forbidden, errors);
        }

        public static Result Forbidden(string field, string error)
        {
            return Forbidden(new List<FieldError> { new FieldError(field, error) });
        }

        //404
        public static Result NotFound(IEnumerable<FieldError> errors)
        {
            return new Result(ResultType.NotFound, errors);
        }

        public static Result NotFound(string field, string error)
        {
            return NotFound(new List<FieldError> { new FieldError(field, error) });
        }
    }

    public class FieldError
    {
        public string Field { get; set; }
        public string Error { get; set; }

        public FieldError(string field, string error)
        {
            Field = field;
            Error = error;
        }
    }

    public class ResultErrors
    {
        public List<FieldError> Errors { get; set; }

        public ResultErrors(List<FieldError> errors)
        {
            Errors = errors;
        }
    }
}
