using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    internal class Moves
    {
        FigureMoving fm;
        Board board;

        internal Moves(Board board)
        {
            this.board = board;
        }

        internal bool CanMove(FigureMoving fm)
        {
            this.fm = fm;
            return CanMoveFrom() && CanMoveTo() && CanFigureMove() && CanBePromotion();
        }

        private bool CanMoveFrom()
        {
            return this.fm.from.OnBoard() && this.fm.figure == this.board.GetFigureAt(fm.from) && this.fm.figure.GetColor() == this.board.moveColor;
        }

        private bool CanMoveTo()
        {
            return this.fm.from.OnBoard() && this.fm.from != this.fm.to && this.board.GetFigureAt(this.fm.to).GetColor() != this.board.moveColor;
        }

        private bool CanFigureMove()
        {
            switch (fm.figure)
            {
                case Figure.whiteKing:
                case Figure.blackKing:
                    return CanKingMove();

                case Figure.whiteQueen:
                case Figure.blackQueen:
                    return CanQueenMove();

                case Figure.whiteRook:
                case Figure.blackRook:
                    return CanRookMove();

                case Figure.whiteBishop:
                case Figure.blackBishop:
                    return CanBishopMove();

                case Figure.whiteKnight:
                case Figure.blackKnight:
                    return CanKnightMove();

                case Figure.whitePawn:
                case Figure.blackPawn:
                    return CanPawnMove();

                default: return false;
            }
        }

        private bool CanBePromotion()
        {
            if (PromotionCondition())
                return true;
            return fm.promotion == Figure.none ? true : false;
        }

        private bool PromotionCondition()
        {
            return (this.fm.figure == Figure.whitePawn && this.fm.from.y == 6) || (this.fm.figure == Figure.blackPawn && this.fm.from.y == 1);
        }

        private bool CanKingMove()
        {
            return CanKingGo() || CanKingsideCastling(this.fm.figure.GetColor()) || CanQueensideCastling(this.fm.figure.GetColor());
        }

        private bool CanKingGo()
        {
            if (this.fm.AbsDeltaX <= 1 && this.fm.AbsDeltaY <= 1)
                return true;
            return false;
        }

        private bool CanKingsideCastling(Color color)
        {
            return color == Color.white ? CanWhiteKingsideCastling() : CanBlackKingsideCastling();
        }

        private bool CanWhiteKingsideCastling()
        {
            if (this.fm.to == new Square("g1"))
                if (this.board.castling.Contains(Figure.whiteKing))
                    if (this.board.GetFigureAt(new Square("f1")) == Figure.none)
                        return true;
            return false;
        }

        private bool CanBlackKingsideCastling()
        {
            if (fm.to == new Square("g8"))
                if (this.board.castling.Contains(Figure.blackKing))
                    if (this.board.GetFigureAt(new Square("f8")) == Figure.none)
                        return true;
            return false;
        }

        private bool CanQueensideCastling(Color color)
        {
            return color == Color.white ? CanWhiteQueensideCastling() : CanBlackQueensideCastling();
        }

        private bool CanWhiteQueensideCastling()
        {
            if (fm.to == new Square("c1"))
                if (this.board.castling.Contains(Figure.whiteQueen))
                    if (this.board.GetFigureAt(new Square("b1")) == Figure.none && this.board.GetFigureAt(new Square("d1")) == Figure.none)
                        return true;
            return false;
        }

        private bool CanBlackQueensideCastling()
        {
            if (fm.to == new Square("c8"))
                if (this.board.castling.Contains(Figure.blackQueen))
                    if (this.board.GetFigureAt(new Square("b8")) == Figure.none && this.board.GetFigureAt(new Square("d8")) == Figure.none)
                        return true;
            return false;
        }

        private bool CanQueenMove()
        {
            Square at = this.fm.from;
            do
            {
                at = new Square(at.x + this.fm.SignDeltaX, at.y + this.fm.SignDeltaY);
                if (at == this.fm.to)
                    return true;
            } while (at.OnBoard() && this.board.GetFigureAt(at) == Figure.none);

            return false;
        }

        private bool CanRookMove()
        {
            return ((this.fm.SignDeltaX == 0 || this.fm.SignDeltaY == 0) && CanQueenMove());
        }

        private bool CanBishopMove()
        {
            return ((this.fm.SignDeltaX != 0 && this.fm.SignDeltaY != 0) && CanQueenMove());
        }

        private bool CanKnightMove()
        {
            if (this.fm.AbsDeltaX == 1 && this.fm.AbsDeltaY == 2)
                return true;
            if (this.fm.AbsDeltaX == 2 && this.fm.AbsDeltaY == 1)
                return true;
            return false;
        }

        private bool CanPawnMove()
        {
            if (this.fm.from.y < 1 || this.fm.from.y > 6)
                return false;
            int stepY = this.fm.figure.GetColor() == Color.white ? 1 : -1;
            return CanPawnGo(stepY) || CanPawnJump(stepY) || CanPawnEat(stepY) || CanPawnEatOnAisle(stepY);
        }

        private bool CanPawnGo(int stepY)
        {
            if (this.board.GetFigureAt(fm.to) == Figure.none)
                if (this.fm.deltaX == 0)
                    if (this.fm.deltaY == stepY)
                        if (PromotionStep() || NoPromotionStep())
                            return true;
            return false;
        }

        private bool CanPawnJump(int stepY)
        {
            if (this.board.GetFigureAt(fm.to) == Figure.none)
                if (this.fm.deltaX == 0)
                    if (this.fm.deltaY == 2 * stepY)
                        if (this.fm.from.y == 1 || this.fm.from.y == 6)
                            if (this.board.GetFigureAt(new Square(this.fm.from.x, this.fm.from.y + stepY)) == Figure.none)
                                return true;                         
            return false;
        }

        private bool CanPawnEat(int stepY)
        {
            if (this.board.GetFigureAt(fm.to) != Figure.none)
                if (this.fm.AbsDeltaX == 1)
                    if (this.fm.deltaY == stepY)
                        if (PromotionStep() || NoPromotionStep())
                            return true;
            return false;
        }

        private bool CanPawnEatOnAisle(int stepY)
        {
            if (fm.to == this.board.pawnPassed)
                if (board.GetFigureAt(fm.to) == Figure.none)
                    if (this.fm.AbsDeltaX == 1)
                        if (this.fm.deltaY == stepY)
                            return true;
            return false;
        }

        private bool NoPromotion()
        {
            return this.fm.promotion == Figure.none ? true : false;
        }

        private bool PromotionStep()
        {
            return PromotionCondition() && fm.promotion != Figure.none;
        }

        private bool NoPromotionStep()
        {
            return !PromotionCondition() && fm.promotion == Figure.none;
        }
    }
}
