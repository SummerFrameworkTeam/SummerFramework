using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SummerFramework.Base;

public static class Extensions
{
    public static string ToLegalName(this DateTime self)
    {
        return self.ToString().Replace(' ', '-').Replace('/', '-').Replace(':', '-');
    }
}
