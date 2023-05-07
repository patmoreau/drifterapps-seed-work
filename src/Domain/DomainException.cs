namespace Holefeeder.Domain.SeedWork;

/// <summary>
///
/// </summary>
public class DomainException : Exception
{
    /// <summary>
    ///
    /// </summary>
    protected DomainException()
    {
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="message"></param>
    protected DomainException(string message) : base(message)
    {
    }


    /// <summary>
    ///
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    protected DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///
    /// </summary>
    public virtual string Context => nameof(DomainException);
}
