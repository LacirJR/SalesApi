namespace Module.Carts.Domain.Exceptions;

public class CartDomainException: Exception
{
    public  CartDomainException(string message) :  base(message){}
}