namespace Module.Products.Domain.Exceptions;

public class ProductDomainException : Exception
{
    public  ProductDomainException(string message) :  base(message){}
}