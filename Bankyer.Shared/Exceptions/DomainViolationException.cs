namespace Bankyer.Shared.Exceptions;

public sealed class DomainViolationException(IReadOnlyCollection<DomainRuleViolation> violations)
    : Exception("One or more domain rules were violated.")
{
    public DomainViolationException(DomainValidation validation) : this(validation.Violations) { }
    
    public IReadOnlyCollection<DomainRuleViolation> Violations { get; } = violations;
}