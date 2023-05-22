using Microsoft.Extensions.Logging;

namespace DrifterApps.Seeds.Application.Mediatr.Tests.Mocks;

public abstract class LoggerMock<T> : ILogger<T>
{
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(formatter);

        var unboxed = (IReadOnlyList<KeyValuePair<string, object>>) state;
        var message = formatter(state, exception);

        Log();
        Log(logLevel, eventId, message, exception);
        Log(logLevel, eventId, unboxed.ToDictionary(k => k.Key, v => v.Value), exception);
    }

    public virtual bool IsEnabled(LogLevel logLevel) => true;

    public abstract IDisposable? BeginScope<TState>(TState state) where TState : notnull;

    public abstract void Log();

    public abstract void Log(LogLevel logLevel, EventId eventId, string message, Exception? exception = null);

    public abstract void Log(LogLevel logLevel, EventId eventId, IDictionary<string, object> state,
        Exception? exception = null);
}
