namespace Givt.Business.Exceptions;

public class BadRequestException: GivtException
{
    public BadRequestException(string message) : base(message) { }

    public override int ErrorCode => 400;
}