using System;


namespace SummerFramework.Core.Configuration.Attributes;
/// <summary>
/// Make you set value for current property in the way of invoking method.
/// Considering performance problem and the readability of codes, it only support method invocations.
/// You can give literal and reference value by equal operators!
/// </summary>
public class SetValueAttribute : Attribute
{
    public string Expression { get; set; }

    public SetValueAttribute(string expr)
    {
        Expression = expr;
    }
}
