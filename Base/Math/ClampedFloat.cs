using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SummerFramework.Base.Math;

public struct ClampedFloat
{
    private float value;
    public float Value { get => this.value; set => this.set_value(value); }
    public float UpperLimit { get; private set; }
    public float LowerLimit { get; private set; }

    public ClampedFloat(float init_value, float ul, float ll)
    {
        this.UpperLimit = ul;
        this.LowerLimit = ll;
        this.value = init_value;
        this.Value = init_value;
    }

    private void set_value(float v)
    {
        if (v >= this.LowerLimit && v <= this.UpperLimit)
            this.Value = v;
        else
            throw new Exception("Out of clamped range");
    }

    public string FormatRange(bool expr = true)
    {
        if (expr)
            return $"{this.LowerLimit} <= x <= {this.UpperLimit}";
        else
            return $"[{this.LowerLimit}, {this.UpperLimit}]";
    }

    // Format: lower_limit <= x <= upper_limit or [lower_limit, upper_limit]
    public static ClampedFloat CreateFromString(string source, float init_value, bool expr = true)
    {
        if (expr)
        {
            var ll = Convert.ToSingle(source.Split(' ')[0]);
            var ul = Convert.ToSingle(source.Split(' ')[4]);
            return new ClampedFloat(init_value, ul, ll);
        }
        else
        {
            var trimed = source.TrimStart('[').TrimEnd(']').Replace(" ", "");
            var ll = Convert.ToSingle(source.Split(',')[0]);
            var ul = Convert.ToSingle(source.Split(',')[1]);
            return new ClampedFloat(init_value, ul, ll);
        }
    }
}
