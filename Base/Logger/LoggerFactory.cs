using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SummerFramework.Base.Logger;

public static class LoggerFactory
{
    public static Logger CreateLogger(string identifier)
    {
        return new Logger(identifier);
    }
}
