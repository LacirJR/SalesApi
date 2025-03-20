namespace Shared.Domain.Common;

/// <summary>
/// All errors contained in ServiceResult objects must return an error of this type
/// Error codes allow the caller to easily identify the received error and take action.
/// Error messages allow the caller to easily show error messages to the end user.
/// </summary>
public class ServiceError
{
    /// <summary>
    /// CTOR
    /// </summary>
    public ServiceError(string type, string error, string detail)
    {
        Exception test = new Exception(error);

        Detail = detail;
        Error = error;
        Type = type;
    }

    public ServiceError()
    {
    }

    /// <summary>
    /// A human-readable explanation specific to this occurrence of the problem
    /// </summary>
    public string Detail { get; }


    /// <summary>
    /// AA short, human-readable summary of the problem
    /// </summary>
    public string Error { get; }

    /// <summary>
    /// Machine readable error code
    /// </summary>
    public string Type { get; }

    
    /// <summary>
    /// Default error for when the requested resource is not found.
    /// </summary>
    public static ServiceError NotFound => new("NotFound", "Resource not found",
        "The requested resource was not found.");
    
    /// <summary>
    /// Default error for when we receive an exception
    /// </summary>
    public static ServiceError DefaultError => new("UnexpectedError", "An unexpected error occurred",
        "An unexpected error occurred. Please try again later.");

    /// <summary>
    /// Default validation error. Use this for invalid parameters in controller actions and service methods.
    /// </summary>
    public static ServiceError ModelStateError(string validationError) => new("ModelStateError",
        "Invalid request. Please check the provided data.",
        validationError);
    
    /// <summary>
    /// Generic Error 
    /// </summary>
    public static ServiceError GenericError(string type, string error, string detail)
    => new(type, error, detail);


    #region Override Equals Operator

    /// <summary>
    /// Use this to compare if two errors are equal
    /// Ref: https://msdn.microsoft.com/ru-ru/library/ms173147(v=vs.80).aspx
    /// </summary>
    public override bool Equals(object obj)
    {
        // If parameter cannot be cast to ServiceError or is null return false.
        var error = obj as ServiceError;

        // Return true if the error codes match. False if the object we're comparing to is nul
        // or if it has a different code.
        return Type == error?.Type;
    }

    public bool Equals(ServiceError error)
    {
        // Return true if the error codes match. False if the object we're comparing to is nul
        // or if it has a different code.
        return Type == error?.Type;
    }

    public static bool operator ==(ServiceError a, ServiceError b)
    {
        // If both are null, or both are same instance, return true.
        if (ReferenceEquals(a, b))
        {
            return true;
        }

        // If one is null, but not both, return false.
        if ((object)a == null || (object)b == null)
        {
            return false;
        }

        // Return true if the fields match:
        return a.Equals(b);
    }

    public static bool operator !=(ServiceError a, ServiceError b)
    {
        return !(a == b);
    }

    #endregion
}