namespace DrifterApps.Seeds.Application;

/// <summary>
///     Query request to enable paging of large results.
/// </summary>
public interface IRequestQuery
{
    /// <summary>
    ///     Offset from 0
    /// </summary>
    int Offset { get; }

    /// <summary>
    ///     Limit of result
    /// </summary>
    int Limit { get; }

#pragma warning disable CA1819
    /// <summary>
    ///     Sort values; use the name of field to sort on. use '-' in front of the field name for descending order.
    /// </summary>
    string[] Sort { get; }

    /// <summary>
    ///     Filter values on fields.
    /// </summary>
    /// <remarks>
    ///     Valid operators:<br />
    ///     eq : equal<br />
    ///     ne : not equal<br />
    ///     gt : greater than<br />
    ///     ge : greater or equal<br />
    ///     lt : lower than<br />
    ///     le : lower or equal
    /// </remarks>
    /// <example>
    ///     field_name:{operator}:value
    /// </example>
    string[] Filter { get; }
#pragma warning restore CA1819
}
