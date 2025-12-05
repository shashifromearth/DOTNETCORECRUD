namespace InterviewAPI.Exceptions;

/// <summary>
/// Custom exception for not found scenarios - demonstrates custom exception handling
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

