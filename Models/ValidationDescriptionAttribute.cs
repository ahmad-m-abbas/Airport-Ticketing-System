namespace Data;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class ValidationDescriptionAttribute : Attribute
{
    public ValidationDescriptionAttribute(string description)
    {
        Description = description;
    }

    public string Description { get; }
}