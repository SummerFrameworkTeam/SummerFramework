using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SummerFramework.Base;

public interface IDictionaryContainer<T>
{
    void Add(string identifier, T item);
    T? Get(string identifer);
}
