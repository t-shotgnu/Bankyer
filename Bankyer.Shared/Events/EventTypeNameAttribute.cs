namespace Bankyer.Shared.Events;

[AttributeUsage(AttributeTargets.Class)]
public class EventTypeNameAttribute(params string[] nameParts) : Attribute
{
    public string[] NameParts => nameParts;
}