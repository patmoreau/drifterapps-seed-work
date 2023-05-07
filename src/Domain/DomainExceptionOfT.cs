namespace Holefeeder.Domain.SeedWork;

/// <inheritdoc />
public class DomainException<TContext> : DomainException
{
    /// <inheritdoc />
    protected DomainException() => Context = typeof(TContext).Name;

    /// <inheritdoc />
    protected DomainException(string message) : base(message) => Context = typeof(TContext).Name;

    /// <inheritdoc />
    protected DomainException(string message, Exception innerException) : base(message, innerException) =>
        Context = nameof(TContext);

    /// <inheritdoc />
    public override string Context { get; }
}
