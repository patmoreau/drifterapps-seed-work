using System.Text.RegularExpressions;

namespace DrifterApps.Seeds.Testing.Extensions;

/// <summary>
///     Provides extension methods for working with URLs.
/// </summary>
public static partial class UrlExtensions
{
    /// <summary>
    ///     Extracts a GUID from the given URL.
    /// </summary>
    /// <param name="url">The URL to extract the GUID from.</param>
    /// <returns>The extracted GUID.</returns>
    /// <exception cref="ArgumentException">Thrown when no valid GUID is found in the URL.</exception>
    public static Guid ExtractGuidFromUrl(this Uri url)
    {
        ArgumentNullException.ThrowIfNull(url);

        var match = LocationIdRegex().Match(url.AbsoluteUri);
        if (match.Success)
        {
            return Guid.Parse(match.Value);
        }

        throw new ArgumentException("No valid GUID found in the URL.");
    }

    /// <summary>
    ///     Returns a regular expression that matches GUIDs in a URL.
    /// </summary>
    /// <returns>A regular expression for matching GUIDs.</returns>
    [GeneratedRegex(@"[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}")]
    private static partial Regex LocationIdRegex();
}
