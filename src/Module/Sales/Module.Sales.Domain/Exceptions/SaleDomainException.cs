namespace Module.Sales.Domain.Exceptions;

public class SaleDomainException : Exception
{
    public  SaleDomainException(string message) :  base(message){}
}