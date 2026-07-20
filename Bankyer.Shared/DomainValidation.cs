using Bankyer.Shared.Exceptions;

namespace Bankyer.Shared;

public record DomainValidation
{
    private readonly List<DomainRuleViolation> _violations = [];
    public IReadOnlyCollection<DomainRuleViolation> Violations => _violations.AsReadOnly();

    public DomainValidation AddIf(
        bool condition,
        string code,
        string message)
    {
        if (condition)
        {
            _violations.Add(new DomainRuleViolation(code, message));
        }
        return this;
    }

    public bool HasViolations => _violations.Count > 0;
    public bool IsValid => _violations.Count == 0;

    public void ThrowIfInvalid()
    {
        if (_violations.Count > 0)
        {
            throw new DomainViolationException(_violations);
        }
    }
}