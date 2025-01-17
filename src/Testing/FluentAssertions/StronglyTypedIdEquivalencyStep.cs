using DrifterApps.Seeds.Domain;
using FluentAssertions.Equivalency;

namespace FluentAssertions;

/// <summary>
///     Implements a custom equivalency step for comparing strongly-typed IDs.
/// </summary>
public class StronglyTypedIdEquivalencyStep : IEquivalencyStep
{
    /// <summary>
    ///     Handles the comparison of strongly-typed IDs.
    /// </summary>
    /// <param name="comparands">The comparands containing the subject and expectation.</param>
    /// <param name="context">The equivalency validation context.</param>
    /// <param name="valueChildNodes">The nested equivalency validator.</param>
    /// <returns>
    ///     An <see cref="EquivalencyResult" /> indicating whether the comparison was handled or should continue.
    /// </returns>
    public EquivalencyResult Handle(Comparands comparands, IEquivalencyValidationContext context,
        IValidateChildNodeEquivalency valueChildNodes)
    {
        switch (comparands)
        {
            case {Subject: Guid subjectGuid, Expectation: IStronglyTypedId expectedStronglyTypedId}:
                subjectGuid.Should().Be(expectedStronglyTypedId.Value);
                return EquivalencyResult.EquivalencyProven; // Indicating that the comparison is handled
            case {Subject: IStronglyTypedId subjectStronglyTypedId, Expectation: Guid expectedGuid}:
                subjectStronglyTypedId.Value.Should().Be(expectedGuid);
                return EquivalencyResult.EquivalencyProven; // Indicating that the comparison is handled
            default:
                return EquivalencyResult.ContinueWithNext; // Indicating that the comparison is not handled
        }
    }
}
