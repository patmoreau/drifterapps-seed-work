namespace DrifterApps.Seeds.Testing.Drivers;

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
    ///     Constructor used by test framework.
    /// </summary>
    public AuthorityDriver() => GetInstance();

    /// <summary>
    ///     Constructor used by singleton instance.
    /// </summary>
    /// <param name="isInstance"></param>
    /// <exception cref="InvalidOperationException"></exception>
    private AuthorityDriver(bool isInstance)
    {
        if (!isInstance)
        {
            throw new InvalidOperationException("Use GetInstance method to create a singleton instance.");
        }
    }

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
            _instance = new AuthorityDriver(true);
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
