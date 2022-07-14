using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    internal class FigureOnSquare
    {
        internal Figure figure { get; private set; }
        internal Square square { get; private set; }

        internal FigureOnSquare(Figure figure, Square square)
        {
            this.figure = figure;
            this.square = square;
        }
    }
}
