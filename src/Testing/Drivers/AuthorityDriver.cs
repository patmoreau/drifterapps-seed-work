using System.Diagnostics.CodeAnalysis;

namespace DrifterApps.Seeds.Testing.Drivers;

[SuppressMessage("Performance", "CA1861:Avoid constant arrays as arguments")]
public sealed partial class AuthorityDriver : WireMockDriver
{
    /// <summary>
    ///     The authority domain for the JWT token issuer.
    /// </summary>
    public const string AuthorityDomain = "localhost:9096";

    private static readonly SemaphoreSlim Semaphore = new(1, 1);
    private static int _usageCount;
    private static AuthorityDriver? _instance;

    /// <summary>
    ///     Constructor handling the singleton instance creation.
    /// </summary>
    public AuthorityDriver() => GetInstance();

    /// <summary>
    ///     Gets the authority URL for the token issuer.
    /// </summary>
    public static string Authority => $"https://{AuthorityDomain}";

    /// <summary>
    ///     Method to create a singleton instance of the AuthorityDriver.
    /// </summary>
    /// <returns>
    ///     <see cref="AuthorityDriver" />
    /// </returns>
    public static AuthorityDriver GetInstance()
    {
        if (_instance is not null)
        {
            return _instance;
        }

        Semaphore.Wait();
        try
        {
            _instance = new AuthorityDriver();
            return _instance;
        }
        finally
        {
            Semaphore.Release();
        }
    }

    /// <inheritdoc />
    public override async Task InitializeAsync()
    {
        await Semaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            if (_usageCount == 0 && _instance is not null)
            {
                await _instance.InitializeAsync().ConfigureAwait(false);
            }

            _usageCount++;
        }
        finally
        {
            Semaphore.Release();
        }
    }

    /// <inheritdoc />
    public override async Task DisposeAsync()
    {
        await Semaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            _usageCount--;
            if (_usageCount == 0 && _instance is not null)
            {
                await _instance.DisposeAsync().ConfigureAwait(false);
            }
        }
        finally
        {
            Semaphore.Release();
        }
    }
}
