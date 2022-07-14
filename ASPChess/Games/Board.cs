using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    internal class Board
    {
        internal string FEN { get; private set; }
        private Figure[,] figures;
        internal Color moveColor { get; private set; }
        internal List<Figure> castling { get; private set; }
        internal Square pawnPassed { get; private set; }
        internal int moveNumberWithoutCapture { get; private set; }
        internal int moveNumber { get; private set; }

        internal Board (string FEN)
        {
            this.FEN = FEN;
            figures = new Figure[8, 8];
            Initialization();
        }

        private void Initialization()
        {
            // rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1
            // 0                                           1 2    3 4 5
            string[] parts = FEN.Split(' ');
            if (parts.Length != 6) return;
            InitFigures(parts[0]);
            moveColor = (parts[1] == "w") ? Color.white : Color.black;
            InitCastling(parts[2]);
            pawnPassed = (parts[3] == "-") ? new Square(-1, -1) : new Square(parts[3]);
            moveNumberWithoutCapture = int.Parse(parts[4]);
            moveNumber = int.Parse(parts[5]);
        }

        private void InitFigures(string data)
        {
            for (int j = 8; j >= 2; j--)
                data = data.Replace(j.ToString(), (j - 1).ToString() + "1");
            string[] lines = data.Split('/');
            for (int y = 7; y >= 0; y--)
                for (int x = 0; x < 8; x++)
                    figures[x, y] = (Char.IsDigit(lines[7 - y][x]))
                        ? Figure.none
                        : (Figure)lines[7 - y][x];
        }

        private void InitCastling(string data)
        {
            this.castling = new List<Figure>();
            if (data.Length > 4 || data.Length == 0)
            {
                this.castling.Add(Figure.none);
                return;
            }
            foreach (char figure in data)
            {
                if ((Figure)figure == Figure.whiteKing ||
                    (Figure)figure == Figure.blackKing ||
                    (Figure)figure == Figure.whiteQueen ||
                    (Figure)figure == Figure.blackQueen)
                    {
                        this.castling.Add((Figure)figure);
                    }
                else
                {
                    if (data.Length == 1 && figure == '-')
                        this.castling.Add(Figure.none);
                }
            }
            return;
        }

        internal IEnumerable<FigureOnSquare> YieldFigures()
        {
            foreach (Square square in Square.YieldSquares())
                if (GetFigureAt(square).GetColor() == moveColor)
                    yield return new FigureOnSquare(GetFigureAt(square), square);
        }

        internal Figure GetFigureAt(Square square)
        {
            if (square.OnBoard())
                return figures[square.x, square.y];
            return Figure.none;
        }

        private void SetFigureAt(Square square, Figure figure)
        {
            if (square.OnBoard())
            {
                figures[square.x, square.y] = figure;
            }
                
        }

        internal Board Move(FigureMoving fm)
        {
            Board next = new Board(this.FEN);
            bool isSimpleEating = this.GetFigureAt(fm.to) != Figure.none ? true : false;
            bool isPawnMove = (fm.figure == Figure.whitePawn || fm.figure == Figure.blackPawn) ? true : false;
            bool isPawnDoubleStep = isPawnMove && fm.AbsDeltaY == 2 ? true : false;
            bool isPawnEatOnAisle = isPawnMove && fm.AbsDeltaX == 1 && !isSimpleEating;

            next.SetFigureAt(fm.from, Figure.none);
            next.SetFigureAt(fm.to, fm.promotion == Figure.none ? fm.figure : fm.promotion);
            if (this.moveColor == Color.black)
                next.moveNumber++;
            next.moveColor = moveColor.FlipColor();
            next.pawnPassed = isPawnDoubleStep ? new Square(fm.to.x, (fm.to.y + fm.from.y) / 2) : new Square(-1, -1);
            if (isPawnEatOnAisle)
                next.SetFigureAt(new Square(fm.to.x, fm.to.y - fm.SignDeltaY), Figure.none);
            if (!isSimpleEating && !isPawnMove)
                next.moveNumberWithoutCapture++;
            else
                next.moveNumberWithoutCapture = 0;
            next.GenerateFEN();
            return next;
        }

        private void GenerateFEN()
        {
            this.FEN =  FenFigures() + " " +
                        FenColor() + " " +
                        FenCastling() + " " +
                        pawnPassed.ToString() + " " +
                        moveNumberWithoutCapture.ToString() + " " +
                        moveNumber.ToString();
        }

        private string FenFigures()
        {
            StringBuilder sb = new StringBuilder();
            for (int y = 7; y >= 0; y--)
            {
                for (int x = 0; x < 8; x++)
                    sb.Append((figures[x, y] == Figure.none) ? '1' : (char)figures[x, y]);
                if (y > 0)
                    sb.Append('/');
            }
            for (int j = 8; j >= 2; j--)
                sb.Replace("11111111".Substring(0, j), j.ToString());
            return sb.ToString();
        }

        private string FenColor()
        {
            return (this.moveColor == Color.white) ? "w" : "b";
        }

        private string FenCastling()
        {
            StringBuilder sb = new StringBuilder();
            if (this.castling.Count == 1 && this.castling[0] == Figure.none)
                return "-";
            foreach (Figure figure in this.castling)
            {
                sb.Append((char)figure);
            }
            return sb.ToString();
        }

        internal Square FindBadKing()
        {
            Figure badKing = moveColor == Color.white ? Figure.blackKing : Figure.whiteKing;
            foreach (Square square in Square.YieldSquares())
                if (GetFigureAt(square) == badKing)
                    return square;
            return new Square(-1, -1);
        }

        internal bool CanEatKing()
        {
            Square badKing = FindBadKing();
            Moves moves = new Moves(this);
            foreach (FigureOnSquare fs in YieldFigures())
            {
                FigureMoving fm = new FigureMoving(fs, badKing);
                if (moves.CanMove(fm))
                    return true;
            }
            return false;
        }

        internal bool IsCheck()
        {
            Board after = new Board(this.FEN);
            after.moveColor = moveColor.FlipColor();
            return after.CanEatKing();
        }

        internal bool IsCheckAfterMove(FigureMoving fm)
        {
            Board after = Move(fm);
            return after.CanEatKing();
        }

        internal void CheckCastling(FigureMoving fm)
        {
            CheckCastlingWitoutFEN(fm);
            this.GenerateFEN();
        }

        private void CheckCastlingWitoutFEN(FigureMoving fm)
        {
            switch (fm.figure)
            {
                case Figure.whiteRook:
                    if (fm.from == new Square("h1"))
                    {
                        this.castling = AuxiliaryMethods.Removed(this.castling, Figure.whiteKing);
                        return;
                    }
                    if (fm.from == new Square("a1"))
                    {
                        this.castling = AuxiliaryMethods.Removed(this.castling, Figure.whiteQueen);
                        return;
                    }
                    return;
                case Figure.blackRook:
                    if (fm.from == new Square("h8"))
                    {
                        this.castling = AuxiliaryMethods.Removed(this.castling, Figure.blackKing);
                        return;
                    }
                    if (fm.from == new Square("a8"))
                    {
                        this.castling = AuxiliaryMethods.Removed(this.castling, Figure.blackQueen);
                        return;
                    }
                    return;
                case Figure.whiteKing:
                    this.castling = AuxiliaryMethods.Removed(this.castling, Figure.whiteKing);
                    this.castling = AuxiliaryMethods.Removed(this.castling, Figure.whiteQueen);
                    if (fm.AbsDeltaY == 0 && fm.AbsDeltaX == 2)
                    {
                        switch (fm.SignDeltaX)
                        {
                            case 1:
                                this.SetFigureAt(new Square("h1"), Figure.none);
                                this.SetFigureAt(new Square("f1"), Figure.whiteRook);
                                return;
                            case -1:
                                this.SetFigureAt(new Square("a1"), Figure.none);
                                this.SetFigureAt(new Square("d1"), Figure.whiteRook);
                                return;
                            default: return;
                        }
                    }
                    return;
                case Figure.blackKing:
                    this.castling = AuxiliaryMethods.Removed(this.castling, Figure.blackKing);
                    this.castling = AuxiliaryMethods.Removed(this.castling, Figure.blackQueen);
                    if (fm.AbsDeltaY == 0 && fm.AbsDeltaX == 2)
                    {
                        switch (fm.SignDeltaX)
                        {
                            case 1:
                                this.SetFigureAt(new Square("h8"), Figure.none);
                                this.SetFigureAt(new Square("f8"), Figure.blackRook);
                                return;
                            case -1:
                                this.SetFigureAt(new Square("a8"), Figure.none);
                                this.SetFigureAt(new Square("d8"), Figure.blackRook);
                                return;
                            default: return;
                        }
                    }
                    return;
                default: return;
            }
        }
    }
}
