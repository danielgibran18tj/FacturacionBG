namespace Application.Common.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string entityName, object key)
            : base($"{entityName} with key '{key}' was not found.")
        {
        }

        public NotFoundException(string message)
            : base(message)
        {
        }
    }


    public class ValidationException : Exception
    {
        public Dictionary<string, string[]> Errors { get; }

        public ValidationException()
            : base("One or more validation errors occurred.")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(Dictionary<string, string[]> errors)
            : this()
        {
            Errors = errors;
        }

        public ValidationException(string field, string error)
            : this()
        {
            Errors.Add(field, new[] { error });
        }
    }


    public class BusinessException : Exception
    {
        public string Code { get; }

        public BusinessException(string message, string code = "BUSINESS_ERROR")
            : base(message)
        {
            Code = code;
        }
    }


    public class UnauthorizedException : Exception
    {
        public UnauthorizedException()
            : base("Unauthorized access.")
        {
        }

        public UnauthorizedException(string message)
            : base(message)
        {
        }
    }


    public class ForbiddenException : Exception
    {
        public ForbiddenException()
            : base("Access forbidden.")
        {
        }

        public ForbiddenException(string message)
            : base(message)
        {
        }
    }


    public class ConflictException : Exception
    {
        public ConflictException(string message)
            : base(message)
        {
        }

        public ConflictException(string entityName, string field, object value)
            : base($"{entityName} with {field} '{value}' already exists.")
        {
        }
    }

}
