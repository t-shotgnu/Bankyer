namespace Bankyer.Shared.Exceptions;

public sealed class DomainViolationException(IReadOnlyCollection<DomainRuleViolation> violations)
    : Exception($"Domain validation failed with {violations.Count} violation(s): {string.Join(", ", violations.Select(v => $"{v.Code}: {v.Message}"))}")
{
    public DomainViolationException(DomainValidation validation) : this(validation.Violations) { }
    
    public IReadOnlyCollection<DomainRuleViolation> Violations { get; } = violations;
}