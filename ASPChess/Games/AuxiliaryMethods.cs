using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    internal class AuxiliaryMethods
    {
        internal static List<Figure> Removed(List<Figure> list, Figure element)
        {
            list.Remove(element);
            if (list.Count == 0)
                list.Add(Figure.none);
            return list;
        }
    }
}
