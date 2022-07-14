using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    internal struct Square
    {
        internal int x { get; private set; }
        internal int y { get; private set; }

        internal Square(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        internal Square(string stringCoord)
        {
            char stringX = stringCoord[0];
            char stringY = stringCoord[1];
            if (stringX >= 'a' && stringX <= 'h' &&
                stringY >= '1' && stringY <= '8')
            {
                this.x = stringX - 'a';
                this.y = stringY - '1';
            }
            else
            {
                this.x = -1;
                this.y = -1;
            }
        }

        internal bool OnBoard()
        {
            return  this.x >= 0 && this.x < 8 &&
                    this.y >= 0 && this.y < 8;
        }

        public string Name { get { return ((char)('a' + x)).ToString() + (y + 1).ToString();  } }

        public override string ToString()
        {
            if (this.x == -1 && this.y == -1)
                return "-";
            StringBuilder sb = new StringBuilder();
            sb.Append((char)(this.x + 'a'));
            sb.Append((char)(this.y + '1'));
            return sb.ToString();
        }

        public static bool operator == (Square a, Square b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=(Square a, Square b)
        {
            return a.x != b.x || a.y != b.y;
        }

        internal static IEnumerable<Square> YieldSquares()
        {
            for (int y = 0; y < 8; y++)
                for (int x = 0; x < 8; x++)
                    yield return new Square(x, y);
        }
    }
}
