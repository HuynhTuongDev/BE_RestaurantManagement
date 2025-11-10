namespace RestaurantManagement.Application.Exceptions;

/// <summary>
/// Exception thrown when a resource is not found
/// </summary>
public class NotFoundException : Exception
{
    /// <summary>
    /// Create not found exception
    /// </summary>
    public NotFoundException(string message) : base(message) { }

    /// <summary>
    /// Create not found exception with resource details
    /// </summary>
    public NotFoundException(string resourceName, int id)
        : base($"{resourceName} with ID {id} not found") { }
}

/// <summary>
/// Exception thrown when validation fails
/// </summary>
public class ValidationException : Exception
{
    /// <summary>
    /// Validation errors dictionary
    /// </summary>
    public Dictionary<string, string[]> Errors { get; }

    /// <summary>
    /// Create validation exception
    /// </summary>
    public ValidationException(string message) : base(message)
    {
        Errors = new Dictionary<string, string[]>();
    }

    /// <summary>
    /// Create validation exception with errors
    /// </summary>
    public ValidationException(Dictionary<string, string[]> errors)
        : base("One or more validation errors occurred")
    {
        Errors = errors;
    }
}

/// <summary>
/// Exception thrown when a resource already exists
/// </summary>
public class ResourceAlreadyExistsException : Exception
{
    /// <summary>
    /// Create resource already exists exception
    /// </summary>
    public ResourceAlreadyExistsException(string message) : base(message) { }

    /// <summary>
    /// Create resource already exists exception with details
    /// </summary>
    public ResourceAlreadyExistsException(string resourceName, string property, string value)
        : base($"{resourceName} with {property} '{value}' already exists") { }
}

/// <summary>
/// Exception thrown when operation is forbidden
/// </summary>
public class ForbiddenException : Exception
{
    /// <summary>
    /// Create forbidden exception
    /// </summary>
    public ForbiddenException(string message) : base(message) { }
}

/// <summary>
/// Exception thrown for unauthorized access
/// </summary>
public class UnauthorizedException : Exception
{
    /// <summary>
    /// Create unauthorized exception
    /// </summary>
    public UnauthorizedException(string message) : base(message) { }
}

/// <summary>
/// Exception thrown when a conflict occurs
/// </summary>
public class ConflictException : Exception
{
    /// <summary>
    /// Create conflict exception
    /// </summary>
    public ConflictException(string message) : base(message) { }
}

/// <summary>
/// Exception thrown for invalid operations
/// </summary>
public class InvalidOperationCustomException : Exception
{
    /// <summary>
    /// Create invalid operation exception
    /// </summary>
    public InvalidOperationCustomException(string message) : base(message) { }
}
