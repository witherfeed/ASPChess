using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    internal class FigureMoving
    {
        internal Figure figure { get; private set; }
        internal Square from { get; private set; }
        internal Square to { get; private set; }
        internal Figure promotion { get; private set; }

        internal FigureMoving(FigureOnSquare fs, Square to, Figure promotion = Figure.none)
        {
            this.figure = fs.figure;
            this.from = fs.square;
            this.to = to;
            this.promotion = promotion;
        }

        internal FigureMoving(string move) //Pe2e4 Ph7h8Q
        {
            this.figure = (Figure)move[0];
            this.from = new Square(move.Substring(1, 2));
            this.to = new Square(move.Substring(3, 2));
            this.promotion = (move.Length == 6) ? (Figure)move[5] : Figure.none;
        }

        internal FigureMoving(FigureMoving fm, Figure promotion = Figure.none)
        {
            this.figure = fm.figure;
            this.from = fm.from;
            this.to = fm.to;
            this.promotion = promotion;
        }

        internal int deltaX { get { return to.x - from.x; } }
        internal int deltaY { get { return to.y - from.y; } }

        internal int AbsDeltaX { get { return Math.Abs(deltaX); } }
        internal int AbsDeltaY { get { return Math.Abs(deltaY); } }

        internal int SignDeltaX { get { return Math.Sign(deltaX); } }
        internal int SignDeltaY { get { return Math.Sign(deltaY); } }

        public override string ToString()
        {
            string text = (char)figure + from.Name + to.Name;
            if (promotion != Figure.none)
                text += (char)promotion;
            return text;
        }
    }
}
