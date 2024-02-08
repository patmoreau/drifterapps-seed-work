namespace DrifterApps.Seeds.Testing.Attributes;

/// <summary>
///     AssertionMethodAttribute Class Indicates that the marked method is assertion method, i.e. it halts control flow if
///     one of the conditions is satisfied. To set the condition, mark one of the parameters with
///     AssertionConditionAttribute attribute
/// </summary>
/// <seealso href="https://rules.sonarsource.com/csharp/RSPEC-2699">SonarQube Spec</seealso>
/// <seealso
///     href="https://www.jetbrains.com/help/resharper/Reference__Code_Annotation_Attributes.html#AssertionMethodAttribute">
///     JetBrain's
///     annotations
/// </seealso>
[AttributeUsage(AttributeTargets.Method)]
public sealed class AssertionMethodAttribute : Attribute;
