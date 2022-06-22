namespace Givt.Business.Exceptions;

public abstract class GivtException : Exception
{
    public Dictionary<string, object> AdditionalInformation { get; }

    public abstract int ErrorCode { get; }
    public GivtExceptionLevel Level { get; set; } = GivtExceptionLevel.Error;

    protected GivtException(string message) : base(message)
    {
        AdditionalInformation = new Dictionary<string, object>();
    }

    public GivtException WithErrorTerm(string term)
    {
        AdditionalInformation.Add("errorTerm", term);
        return this;
    }
}

public enum GivtExceptionLevel
{

    /// <summary>
    /// The lifeblood of operational intelligence - things
    /// happen.
    /// </summary>
    Information,
    /// <summary>
    /// Service is degraded or endangered.
    /// </summary>
    Warning,
    /// <summary>
    /// Functionality is unavailable, invariants are broken
    /// or data is lost.
    /// </summary>
    Error
}