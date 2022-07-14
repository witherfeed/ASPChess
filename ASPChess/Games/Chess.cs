using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    public class Chess
    {
        public string FEN { get; private set; }
        internal Board board;
        internal Moves moves;
        List<FigureMoving> allMoves;
        public Situations Situation { get; private set; }

        public Chess(string FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")
        {
            this.FEN = FEN;
            this.board = new Board(FEN);
            this.moves = new Moves(this.board);
            InitSituation();
        }
        internal Chess(Board board)
        {
            this.board = board;
            this.FEN = board.FEN;
            this.moves = new Moves(board);
            InitSituation();
        }

        private void InitSituation()
        {
            if (IsCheck())
            {
                if (GetAllMoves().Count == 0)
                {
                    this.Situation = Chess.Situations.CheckMate;
                    return;
                }
                this.Situation = Chess.Situations.Check;
                return;
            }
            if (GetAllMoves().Count == 0)
            {
                this.Situation = Chess.Situations.StaleMate;
                return;
            }
            this.Situation = Chess.Situations.none;
            return;
        }

        public Chess Move(string move)
        {
            FigureMoving fm = new FigureMoving(move);
            if (!moves.CanMove(fm))
                return this;
            if (board.IsCheckAfterMove(fm))
                return this;

            Board nextBoard = this.board.Move(fm);
            nextBoard.CheckCastling(fm);

            Chess Next = new Chess(nextBoard);
            return Next;
        }

        public char GetFigureAt(int x, int y)
        {
            Square square = new Square(x, y);
            Figure f = this.board.GetFigureAt(square);
            return f == Figure.none ? '.' : (char)f;
        }

        public string GetColor(char figure)
        {
            if ((Figure)figure == Figure.none)
                return "none";
            return ((Figure)figure).GetColor() == Color.white ? "white" : "black";
        }

        public string GetMoveColor()
        {
            if (this.board.moveColor == Color.none)
                return "none";
            return (this.board.moveColor == Color.white) ? "white" : "black";
        }

        private void FindAllMoves()
        {
            this.allMoves = new List<FigureMoving>();
            foreach (FigureOnSquare fs in this.board.YieldFigures())
            {
                foreach (Square to in Square.YieldSquares())
                {
                    foreach (Figure promotion in GetAllPromotionsByColor(fs.figure.GetColor()))
                    {
                        FigureMoving fm = new FigureMoving(fs, to, promotion);
                        if (this.moves.CanMove(fm))
                        {
                            if (!board.IsCheckAfterMove(fm))
                            {
                                //Console.WriteLine(this.board.moveColor == Color.white);
                                this.allMoves.Add(fm);
                            }
                        }
                    }
                }
            }
        }

        public List<string> GetAllMoves()
        {
            FindAllMoves();
            List<string> list = new List<string>();
            foreach (FigureMoving fm in this.allMoves)
                list.Add(fm.ToString());
            return list;
        }

        public bool IsCheck()
        {
            return this.board.IsCheck();
        }

        internal static List<Figure> GetAllPromotionsByColor(Color color)
        {
            List<Figure> figures = new List<Figure>();
            if (color == Color.white)
            {
                figures.Add(Figure.none);
                figures.Add(Figure.whiteQueen);
                figures.Add(Figure.whiteRook);
                figures.Add(Figure.whiteBishop);
                figures.Add(Figure.whiteKnight);
                return figures;
            }
            if (color == Color.black)
            {
                figures.Add(Figure.none);
                figures.Add(Figure.blackQueen);
                figures.Add(Figure.blackRook);
                figures.Add(Figure.blackBishop);
                figures.Add(Figure.blackKnight);
                return figures;
            }
            figures.Add(Figure.none);
            return figures;
        }
        public enum Situations
        {
            none,

            Check,
            CheckMate,

            StaleMate
        }

        public string StringSituanion { get { return SituationToString(); } }
        private string SituationToString()
        {
            if (this.Situation == Situations.Check)
                return "Check";
            if (this.Situation == Situations.CheckMate)
                return "CheckMate";
            if (this.Situation == Situations.StaleMate)
                return "StaleMate";
            return "";
        }

        public bool IsDraw()
        {
            return this.board.moveNumberWithoutCapture >= 100 ? true : false;
        }
    }
}