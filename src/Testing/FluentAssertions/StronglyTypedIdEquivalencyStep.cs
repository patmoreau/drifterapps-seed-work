using DrifterApps.Seeds.Domain;
using FluentAssertions.Equivalency;

// ReSharper disable once CheckNamespace

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
    /// <param name="nestedValidator">The nested equivalency validator.</param>
    /// <returns>
    ///     An <see cref="EquivalencyResult" /> indicating whether the comparison was handled or should continue.
    /// </returns>
    public EquivalencyResult Handle(Comparands comparands, IEquivalencyValidationContext context,
        IEquivalencyValidator nestedValidator)
    {
        if (comparands is not {Subject: Guid subject, Expectation: IStronglyTypedId expected})
        {
            return EquivalencyResult.ContinueWithNext; // Indicating that the comparison is not handled
        }

        subject.Should().Be(expected.Value);

        return EquivalencyResult.AssertionCompleted; // Indicating that the comparison is handled
    }
}
