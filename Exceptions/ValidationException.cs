namespace InterviewAPI.Exceptions;

/// <summary>
/// Custom exception for validation errors - demonstrates custom exception handling
/// </summary>
public class ValidationException : Exception
{
    public ValidationException(string message) : base(message)
    {
    }

    public ValidationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

