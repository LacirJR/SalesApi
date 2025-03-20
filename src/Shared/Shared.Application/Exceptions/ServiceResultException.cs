using Shared.Domain.Common;

namespace Shared.Application.Exceptions;

public class ServiceResultException : Exception
{
    public ServiceError ServiceError { get; }

    public ServiceResultException(ServiceError serviceError)
        : base(serviceError?.Detail)
    {
        ServiceError = serviceError ?? ServiceError.DefaultError;
    }
}