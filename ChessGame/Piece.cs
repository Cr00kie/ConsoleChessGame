using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace ChessGame
{
    public abstract class Piece
    {
        protected int[] Position;
        public string color;
        public int timesMoved;
        public string icon;
        public Piece(int[] position, string color)
        {
            this.Position = position;
            this.color = color;
            timesMoved = 0;
            icon = "**";
        }
        public virtual int[,] GetPosibleMoves(Piece[,] chessBoard)
        {
            return null;
        }
        static public bool IsUnderCheck(Piece[,] chessBoard, int[] selSquare, int[] newSquare, string player)
        {
            chessBoard[newSquare[0], newSquare[1]] = chessBoard[selSquare[0], selSquare[1]];

            string colorEmptySquare = (selSquare[0] + selSquare[1]) % 2 == 0 ? "white" : "black";

            chessBoard[selSquare[0], selSquare[1]] =
                new EmptyPiece(chessBoard[selSquare[0], selSquare[1]].GetPosition(), colorEmptySquare);

            chessBoard[newSquare[0], newSquare[1]].SetPosition(newSquare);

            int[] kingSquare = null;

            foreach (Piece piece in chessBoard) //Finds the player king
            {
                if (piece.color == player && piece.GetType() == typeof(King))
                {
                    kingSquare = piece.GetPosition();
                    break;
                }
            }
            if (kingSquare == null)
            {
                throw new Exception($"error: couldnt find {player}'s king");
            }

            foreach (Piece piece in chessBoard) //Sees if an enemy piece threatens the king
            {
                if (piece.color == player || piece.GetType() == typeof(EmptyPiece)) //skips ally piece and empty squares
                {
                    continue;
                }

                int[,] pieceMoves = piece.GetPosibleMoves(chessBoard);

                for (int i = 0; i < pieceMoves.GetLength(0); i++) //sees if a move from said piece can take the king
                {
                    if (pieceMoves[i, 0] == kingSquare[0] && pieceMoves[i, 1] == kingSquare[1])
                    {

                        //Changes the board back to how it was
                        chessBoard[selSquare[0], selSquare[1]] = chessBoard[newSquare[0], newSquare[1]];

                        colorEmptySquare = (newSquare[0] + newSquare[1]) % 2 == 0 ? "white" : "black";

                        chessBoard[newSquare[0], newSquare[1]] =
                            new EmptyPiece(chessBoard[newSquare[0], newSquare[1]].GetPosition(), colorEmptySquare);

                        chessBoard[selSquare[0], selSquare[1]].SetPosition(selSquare);

                        Console.WriteLine("error: Your king would be or is under check");
                        return true;
                    }
                }

            }
            //Changes the board back to how it was
            chessBoard[selSquare[0], selSquare[1]] = chessBoard[newSquare[0], newSquare[1]];

            colorEmptySquare = (newSquare[0] + newSquare[1]) % 2 == 0 ? "white" : "black";

            chessBoard[newSquare[0], newSquare[1]] =
                new EmptyPiece(chessBoard[newSquare[0], newSquare[1]].GetPosition(), colorEmptySquare);

            chessBoard[selSquare[0], selSquare[1]].SetPosition(selSquare);

            return false;
        }
        public void SetPosition(int[] aPosition)
        {
            Position = aPosition;
            timesMoved++;
        }
        public int[] GetPosition()
        {
            return Position;
        }


    }
    public class EmptyPiece : Piece
    {
        public EmptyPiece(int[] position, string color) : base(position, color)
        {
            if (color == "white") icon = "##"; else icon = "--";
        }

    }
    public class Pawn : Piece
    {
        public Pawn(int[] position, string color) : base(position, color)
        {
            if (color == "white") icon = "wP"; else icon = "bP";
        }
        public override int[,] GetPosibleMoves(Piece[,] chessBoard)
        {
            List<int[]> posibleMoves = new List<int[]>();

            if (this.color == "white")
            {
                if (chessBoard[this.Position[0] - 1, this.Position[1]].GetType() == typeof(EmptyPiece))
                {
                    posibleMoves.Add(new int[] { this.Position[0] - 1, this.Position[1] });
                    if (chessBoard[this.Position[0] - 2, this.Position[1]].GetType() == typeof(EmptyPiece) && this.timesMoved == 0)
                    {
                        posibleMoves.Add(new int[] { this.Position[0] - 2, this.Position[1] });
                    }
                }
                if (this.Position[1] != 0)
                {
                    if (chessBoard[this.Position[0] - 1, this.Position[1] - 1].GetType() != typeof(EmptyPiece)
                        && chessBoard[this.Position[0] - 1, this.Position[1] - 1].color != this.color)
                    {
                        posibleMoves.Add(new int[] { this.Position[0] - 1, this.Position[1] - 1 });
                    }
                }
                if (this.Position[1] != 7)
                {
                    if (chessBoard[this.Position[0] - 1, this.Position[1] + 1].GetType() != typeof(EmptyPiece)
                        && chessBoard[this.Position[0] - 1, this.Position[1] + 1].color != this.color)
                    {
                        posibleMoves.Add(new int[] { this.Position[0] - 1, this.Position[1] + 1 });
                    }
                }


            }

            if (this.color == "black")
            {
                if (chessBoard[this.Position[0] + 1, this.Position[1]].GetType() == typeof(EmptyPiece))
                {
                    posibleMoves.Add(new int[] { this.Position[0] + 1, this.Position[1] });
                    if (chessBoard[this.Position[0] + 2, this.Position[1]].GetType() == typeof(EmptyPiece) && this.timesMoved == 0)
                    {
                        posibleMoves.Add(new int[] { this.Position[0] + 2, this.Position[1] });
                    }
                }
                if (this.Position[1] != 0)
                {
                    if (chessBoard[this.Position[0] + 1, this.Position[1] - 1].GetType() != typeof(EmptyPiece)
                        && chessBoard[this.Position[0] + 1, this.Position[1] - 1].color != this.color)
                    {
                        posibleMoves.Add(new int[] { this.Position[0] + 1, this.Position[1] - 1 });
                    }
                }
                if (this.Position[1] != 7)
                {
                    if (chessBoard[this.Position[0] + 1, this.Position[1] + 1].GetType() != typeof(EmptyPiece)
                        && chessBoard[this.Position[0] + 1, this.Position[1] + 1].color != this.color)
                    {
                        posibleMoves.Add(new int[] { this.Position[0] + 1, this.Position[1] + 1 });
                    }
                }


            }

            return Tools.CreateRectangularArray(posibleMoves);
        }
    }
    public class Horse : Piece
    {
        public Horse(int[] position, string color) : base(position, color)
        {
            if (color == "white") icon = "wH"; else icon = "bH";
        }
        public override int[,] GetPosibleMoves(Piece[,] chessBoard)
        {
            List<int[]> posibleMoves = new List<int[]>();

            // up
            if (this.Position[0] - 2 >= 0)
            {
                //right
                if (this.Position[1] != 7)
                {
                    if (chessBoard[this.Position[0] - 2, this.Position[1] + 1].GetType() == typeof(EmptyPiece))
                    {
                        posibleMoves.Add(new int[] { this.Position[0] - 2, this.Position[1] + 1 });
                    }
                    else if(chessBoard[this.Position[0] - 2, this.Position[1] + 1].color != this.color)
                    {
                        posibleMoves.Add(new int[] { this.Position[0] - 2, this.Position[1] + 1 });
                    }
                }

                //left
                if (this.Position[1] != 0)
                {
                    if (chessBoard[this.Position[0] - 2, this.Position[1] - 1].GetType() == typeof(EmptyPiece))
                    {
                        posibleMoves.Add(new int[] { this.Position[0] - 2, this.Position[1] - 1 });
                    }
                    else if (chessBoard[this.Position[0] - 2, this.Position[1] - 1].color != this.color)
                    {
                        posibleMoves.Add(new int[] { this.Position[0] - 2, this.Position[1] - 1 });
                    }
                }
            }

            // down
            if (this.Position[0] + 2 <= 7)
            {
                //right
                if (this.Position[1] != 7)
                {
                    if (chessBoard[this.Position[0] + 2, this.Position[1] + 1].GetType() == typeof(EmptyPiece))
                    {
                        posibleMoves.Add(new int[] { this.Position[0] + 2, this.Position[1] + 1 });
                    }
                    else if (chessBoard[this.Position[0] + 2, this.Position[1] + 1].color != this.color)
                    {
                        posibleMoves.Add(new int[] { this.Position[0] + 2, this.Position[1] + 1 });
                    }
                }

                //left
                if (this.Position[1] != 0)
                {
                    if (chessBoard[this.Position[0] + 2, this.Position[1] - 1].GetType() == typeof(EmptyPiece))
                    {
                        posibleMoves.Add(new int[] { this.Position[0] + 2, this.Position[1] - 1 });
                    }
                    else if (chessBoard[this.Position[0] + 2, this.Position[1] - 1].color != this.color)
                    {
                        posibleMoves.Add(new int[] { this.Position[0] + 2, this.Position[1] - 1 });
                    }
                }
            }

            // right
            if (this.Position[1] + 2 <= 7)
            {
                //up
                if (this.Position[0] != 0)
                {
                    if (chessBoard[this.Position[0] - 1, this.Position[1] + 2].GetType() == typeof(EmptyPiece))
                    {
                        posibleMoves.Add(new int[] { this.Position[0] - 1, this.Position[1] + 2 });
                    }
                    else if (chessBoard[this.Position[0] - 1, this.Position[1] + 2].color != this.color)
                    {
                        posibleMoves.Add(new int[] { this.Position[0] - 1, this.Position[1] + 2 });
                    }
                }

                //down
                if (this.Position[0] != 7)
                {
                    if (chessBoard[this.Position[0] + 1, this.Position[1] + 2].GetType() == typeof(EmptyPiece))
                    {
                        posibleMoves.Add(new int[] { this.Position[0] + 1, this.Position[1] + 2 });
                    }
                    else if (chessBoard[this.Position[0] + 1, this.Position[1] + 2].color != this.color)
                    {
                        posibleMoves.Add(new int[] { this.Position[0] + 1, this.Position[1] + 2 });
                    }
                }
            }

            // left
            if (this.Position[1] - 2 >= 0)
            {
                //up
                if (this.Position[0] != 0)
                {
                    if (chessBoard[this.Position[0] - 1, this.Position[1] - 2].GetType() == typeof(EmptyPiece))
                    {
                        posibleMoves.Add(new int[] { this.Position[0] - 1, this.Position[1] - 2 });
                    }
                    else if (chessBoard[this.Position[0] - 1, this.Position[1] - 2].color != this.color)
                    {
                        posibleMoves.Add(new int[] { this.Position[0] - 1, this.Position[1] - 2 });
                    }
                }

                //down
                if (this.Position[0] != 7)
                {
                    if (chessBoard[this.Position[0] + 1, this.Position[1] - 2].GetType() == typeof(EmptyPiece))
                    {
                        posibleMoves.Add(new int[] { this.Position[0] + 1, this.Position[1] - 2 });
                    }
                    else if (chessBoard[this.Position[0] + 1, this.Position[1] - 2].color != this.color)
                    {
                        posibleMoves.Add(new int[] { this.Position[0] + 1, this.Position[1] - 2 });
                    }
                }
            }

            return Tools.CreateRectangularArray(posibleMoves);
        }
        
    }
    public class Bishop : Piece
    {
        public Bishop(int[] position, string color) : base(position, color)
        {
            if (color == "white") icon = "wB"; else icon = "bB";
        }
        public override int[,] GetPosibleMoves(Piece[,] chessBoard)
        {
            List<int[]> posibleMoves = new List<int[]>();

            //diagonal up left
            int index = 1;
            do
            {
                if (this.Position[0] == 0 || this.Position[1] == 0 ||
                    this.Position[0] - index <= -1 || this.Position[1] - index <= -1)
                {
                    break;
                }
                if (chessBoard[this.Position[0] - index, this.Position[1] - index].GetType() != typeof(EmptyPiece))
                {
                    if (chessBoard[this.Position[0] - index, this.Position[1] - index].color != this.color)
                    {
                        posibleMoves.Add(new int[] { this.Position[0] - index, this.Position[1] - index });
                        break;
                    }
                    else
                    {
                        break;
                    }
                }

                posibleMoves.Add(new int[] { this.Position[0] - index, this.Position[1] - index });

                index++;

            } while (true);

            //diagonal up right
            index = 1;
            do
            {
                if (this.Position[0] == 0 || this.Position[1] == 7 || 
                    this.Position[0] - index <= -1 || this.Position[1] + index >= 8)
                {
                    break;
                }
                if (chessBoard[this.Position[0] - index, this.Position[1] + index].GetType() != typeof(EmptyPiece))
                {
                    if (chessBoard[this.Position[0] - index, this.Position[1] + index].color != this.color)
                    {
                        posibleMoves.Add(new int[] { this.Position[0] - index, this.Position[1] + index });
                        break;
                    }
                    else
                    {
                        break;
                    }
                }

                posibleMoves.Add(new int[] { this.Position[0] - index, this.Position[1] + index });

                index++;

            } while (true);

            //diagonal down left
            index = 1;
            do
            {
                if (this.Position[0] == 7 || this.Position[1] == 0 ||
                    this.Position[0] + index >= 8 || this.Position[1] - index <= -1)
                {
                    break;
                }
                if (chessBoard[this.Position[0] + index, this.Position[1] - index].GetType() != typeof(EmptyPiece))
                {
                    if (chessBoard[this.Position[0] + index, this.Position[1] - index].color != this.color)
                    {
                        posibleMoves.Add(new int[] { this.Position[0] + index, this.Position[1] - index });
                        break;
                    }
                    else
                    {
                        break;
                    }
                }

                posibleMoves.Add(new int[] { this.Position[0] + index, this.Position[1] - index });

                index++;

            } while (true);

            //diagonal down right
            index = 1;
            do
            {
                if (this.Position[0] == 7 || this.Position[1] == 7 ||
                    this.Position[0] + index >= 8 || this.Position[1] + index >= 8)
                {
                    break;
                }
                if (chessBoard[this.Position[0] + index, this.Position[1] + index].GetType() != typeof(EmptyPiece))
                {
                    if (chessBoard[this.Position[0] + index, this.Position[1] + index].color != this.color)
                    {
                        posibleMoves.Add(new int[] { this.Position[0] + index, this.Position[1] + index });
                        break;
                    }
                    else
                    {
                        break;
                    }
                }

                posibleMoves.Add(new int[] { this.Position[0] + index, this.Position[1] + index });

                index++;

            } while (true);

            return Tools.CreateRectangularArray(posibleMoves);
        }
    }
    public class Rook : Piece
    {
        public Rook(int[] position, string color) : base(position, color)
        {
            if (color == "white") icon = "wR"; else icon = "bR";
        }
        public override int[,] GetPosibleMoves(Piece[,] chessBoard)
        {
            List<int[]> posibleMoves = new List<int[]>();

            //up
            int index = 1;
            do
            {
                if (this.Position[0] == 0 || this.Position[0] - index <= -1)
                {
                    break;
                }
                if (chessBoard[this.Position[0] - index, this.Position[1]].GetType() != typeof(EmptyPiece))
                {
                    if (chessBoard[this.Position[0] - index, this.Position[1]].color != this.color)
                    {
                        posibleMoves.Add(new int[] { this.Position[0] - index, this.Position[1] });
                        break;
                    }
                    else
                    {
                        break;
                    }
                }

                posibleMoves.Add(new int[] { this.Position[0] - index, this.Position[1] });

                index++;

            } while (true);

            //right
            index = 1;
            do
            {
                if (this.Position[1] == 7 || this.Position[1] + index >= 8)
                {
                    break;
                }
                if (chessBoard[this.Position[0], this.Position[1] + index].GetType() != typeof(EmptyPiece))
                {
                    if (chessBoard[this.Position[0], this.Position[1] + index].color != this.color)
                    {
                        posibleMoves.Add(new int[] { this.Position[0], this.Position[1] + index });
                        break;
                    }
                    else
                    {
                        break;
                    }
                }

                posibleMoves.Add(new int[] { this.Position[0], this.Position[1] + index });

                index++;

            } while (true);

            //down
            index = 1;
            do
            {
                if (this.Position[0] == 7 || this.Position[0] + index >= 8)
                {
                    break;
                }
                if (chessBoard[this.Position[0] + index, this.Position[1]].GetType() != typeof(EmptyPiece))
                {
                    if (chessBoard[this.Position[0] + index, this.Position[1]].color != this.color)
                    {
                        posibleMoves.Add(new int[] { this.Position[0] + index, this.Position[1] });
                        break;
                    }
                    else
                    {
                        break;
                    }
                }

                posibleMoves.Add(new int[] { this.Position[0] + index, this.Position[1] });

                index++;

            } while (true);

            //left
            index = 1;
            do
            {
                if (this.Position[1] == 0 || this.Position[1] - index <= -1)
                {
                    break;
                }
                if (chessBoard[this.Position[0], this.Position[1] - index].GetType() != typeof(EmptyPiece))
                {
                    if (chessBoard[this.Position[0], this.Position[1] - index].color != this.color)
                    {
                        posibleMoves.Add(new int[] { this.Position[0], this.Position[1] - index });
                        break;
                    }
                    else
                    {
                        break;
                    }
                }

                posibleMoves.Add(new int[] { this.Position[0], this.Position[1] - index });

                index++;

            } while (true);

            return Tools.CreateRectangularArray(posibleMoves);
        }
    }
    public class Queen : Piece
    {
        public Queen(int[] position, string color) : base(position, color)
        {
            if (color == "white") icon = "wQ"; else icon = "bQ";
        }
        public override int[,] GetPosibleMoves(Piece[,] chessBoard)
        {
            List<int[]> posibleMoves = new List<int[]>();

            //up
            int index = 1;
            do
            {
                if (this.Position[0] == 0 || this.Position[0] - index <= -1)
                {
                    break;
                }
                if (chessBoard[this.Position[0] - index, this.Position[1]].GetType() != typeof(EmptyPiece))
                {
                    if (chessBoard[this.Position[0] - index, this.Position[1]].color != this.color)
                    {
                        posibleMoves.Add(new int[] { this.Position[0] - index, this.Position[1] });
                        break;
                    }
                    else
                    {
                        break;
                    }
                }

                posibleMoves.Add(new int[] { this.Position[0] - index, this.Position[1] });

                index++;

            } while (true);

            //right
            index = 1;
            do
            {
                if (this.Position[1] == 7 || this.Position[1] + index >= 8)
                {
                    break;
                }
                if (chessBoard[this.Position[0], this.Position[1] + index].GetType() != typeof(EmptyPiece))
                {
                    if (chessBoard[this.Position[0], this.Position[1] + index].color != this.color)
                    {
                        posibleMoves.Add(new int[] { this.Position[0], this.Position[1] + index });
                        break;
                    }
                    else
                    {
                        break;
                    }
                }

                posibleMoves.Add(new int[] { this.Position[0], this.Position[1] + index });

                index++;

            } while (true);

            //down
            index = 1;
            do
            {
                if (this.Position[0] == 7 || this.Position[0] + index >= 8)
                {
                    break;
                }
                if (chessBoard[this.Position[0] + index, this.Position[1]].GetType() != typeof(EmptyPiece))
                {
                    if (chessBoard[this.Position[0] + index, this.Position[1]].color != this.color)
                    {
                        posibleMoves.Add(new int[] { this.Position[0] + index, this.Position[1] });
                        break;
                    }
                    else
                    {
                        break;
                    }
                }

                posibleMoves.Add(new int[] { this.Position[0] + index, this.Position[1] });

                index++;

            } while (true);

            //left
            index = 1;
            do
            {
                if (this.Position[1] == 0 || this.Position[1] - index <= -1)
                {
                    break;
                }
                if (chessBoard[this.Position[0], this.Position[1] - index].GetType() != typeof(EmptyPiece))
                {
                    if (chessBoard[this.Position[0], this.Position[1] - index].color != this.color)
                    {
                        posibleMoves.Add(new int[] { this.Position[0], this.Position[1] - index });
                        break;
                    }
                    else
                    {
                        break;
                    }
                }

                posibleMoves.Add(new int[] { this.Position[0], this.Position[1] - index });

                index++;

            } while (true);

            //diagonal up left
            index = 1;
            do
            {
                if (this.Position[0] == 0 || this.Position[1] == 0 ||
                    this.Position[0] - index <= -1 || this.Position[1] - index <= -1)
                {
                    break;
                }
                if (chessBoard[this.Position[0] - index, this.Position[1] - index].GetType() != typeof(EmptyPiece))
                {
                    if (chessBoard[this.Position[0] - index, this.Position[1] - index].color != this.color)
                    {
                        posibleMoves.Add(new int[] { this.Position[0] - index, this.Position[1] - index });
                        break;
                    }
                    else
                    {
                        break;
                    }
                }

                posibleMoves.Add(new int[] { this.Position[0] - index, this.Position[1] - index });

                index++;

            } while (true);

            //diagonal up right
            index = 1;
            do
            {
                if (this.Position[0] == 0 || this.Position[1] == 7 ||
                    this.Position[0] - index <= -1 || this.Position[1] + index >= 8)
                {
                    break;
                }
                if (chessBoard[this.Position[0] - index, this.Position[1] + index].GetType() != typeof(EmptyPiece))
                {
                    if (chessBoard[this.Position[0] - index, this.Position[1] + index].color != this.color)
                    {
                        posibleMoves.Add(new int[] { this.Position[0] - index, this.Position[1] + index });
                        break;
                    }
                    else
                    {
                        break;
                    }
                }

                posibleMoves.Add(new int[] { this.Position[0] - index, this.Position[1] + index });

                index++;

            } while (true);

            //diagonal down left
            index = 1;
            do
            {
                if (this.Position[0] == 7 || this.Position[1] == 0 ||
                    this.Position[0] + index >= 8 || this.Position[1] - index <= -1)
                {
                    break;
                }
                if (chessBoard[this.Position[0] + index, this.Position[1] - index].GetType() != typeof(EmptyPiece))
                {
                    if (chessBoard[this.Position[0] + index, this.Position[1] - index].color != this.color)
                    {
                        posibleMoves.Add(new int[] { this.Position[0] + index, this.Position[1] - index });
                        break;
                    }
                    else
                    {
                        break;
                    }
                }

                posibleMoves.Add(new int[] { this.Position[0] + index, this.Position[1] - index });

                index++;

            } while (true);

            //diagonal down right
            index = 1;
            do
            {
                if (this.Position[0] == 7 || this.Position[1] == 7 ||
                    this.Position[0] + index >= 8 || this.Position[1] + index >= 8)
                {
                    break;
                }
                if (chessBoard[this.Position[0] + index, this.Position[1] + index].GetType() != typeof(EmptyPiece))
                {
                    if (chessBoard[this.Position[0] + index, this.Position[1] + index].color != this.color)
                    {
                        posibleMoves.Add(new int[] { this.Position[0] + index, this.Position[1] + index });
                        break;
                    }
                    else
                    {
                        break;
                    }
                }

                posibleMoves.Add(new int[] { this.Position[0] + index, this.Position[1] + index });

                index++;

            } while (true);

            return Tools.CreateRectangularArray(posibleMoves);
        }
    }
    public class King : Piece
    {
        public King(int[] position, string color) : base(position, color)
        {
            if (color == "white") icon = "wK"; else icon = "bK";
        }

        public override int[,] GetPosibleMoves(Piece[,] chessBoard)
        {
            List<int[]> posibleMoves = new List<int[]>();

            //up
            if (this.Position[0] != 0)
            {
                if (chessBoard[this.Position[0] - 1, this.Position[1]].GetType() == typeof(EmptyPiece))
                {
                    posibleMoves.Add(new int[] { this.Position[0] - 1, this.Position[1] });
                }
                else if(chessBoard[this.Position[0] - 1, this.Position[1]].color != this.color)
                {
                    posibleMoves.Add(new int[] { this.Position[0] - 1, this.Position[1] });
                }
            }

            //down
            if (this.Position[0] != 7)
            {
                if (chessBoard[this.Position[0] + 1, this.Position[1]].GetType() == typeof(EmptyPiece))
                {
                    posibleMoves.Add(new int[] { this.Position[0] + 1, this.Position[1] });
                }
                else if (chessBoard[this.Position[0] + 1, this.Position[1]].color != this.color)
                {
                    posibleMoves.Add(new int[] { this.Position[0] + 1, this.Position[1] });
                }
            }

            //right
            if (this.Position[1] != 7)
            {
                if (chessBoard[this.Position[0], this.Position[1] + 1].GetType() == typeof(EmptyPiece))
                {
                    posibleMoves.Add(new int[] { this.Position[0], this.Position[1] + 1 });
                }
                else if (chessBoard[this.Position[0], this.Position[1] + 1].color != this.color)
                {
                    posibleMoves.Add(new int[] { this.Position[0], this.Position[1] + 1 });
                }
            }

            //left
            if (this.Position[1] != 0)
            {
                if (chessBoard[this.Position[0], this.Position[1] - 1].GetType() == typeof(EmptyPiece))
                {
                    posibleMoves.Add(new int[] { this.Position[0], this.Position[1] - 1 });
                }
                else if (chessBoard[this.Position[0], this.Position[1] - 1].color != this.color)
                {
                    posibleMoves.Add(new int[] { this.Position[0], this.Position[1] - 1 });
                }
            }

            // diagonal up left
            if (this.Position[0] != 0 && this.Position[1] != 0)
            {
                if (chessBoard[this.Position[0] - 1, this.Position[1] - 1].GetType() == typeof(EmptyPiece))
                {
                    posibleMoves.Add(new int[] { this.Position[0] - 1, this.Position[1] - 1 });
                }
                else if (chessBoard[this.Position[0] - 1, this.Position[1] - 1].color != this.color)
                {
                    posibleMoves.Add(new int[] { this.Position[0] - 1, this.Position[1] - 1 });
                }
            }

            // diagonal up right
            if (this.Position[0] != 0 && this.Position[1] != 7)
            {
                if (chessBoard[this.Position[0] - 1, this.Position[1] + 1].GetType() == typeof(EmptyPiece))
                {
                    posibleMoves.Add(new int[] { this.Position[0] - 1, this.Position[1] + 1 });
                }
                else if (chessBoard[this.Position[0] - 1, this.Position[1] + 1].color != this.color)
                {
                    posibleMoves.Add(new int[] { this.Position[0] - 1, this.Position[1] + 1 });
                }
            }

            // diagonal down left
            if (this.Position[0] != 7 && this.Position[1] != 0)
            {
                if (chessBoard[this.Position[0] + 1, this.Position[1] - 1].GetType() == typeof(EmptyPiece))
                {
                    posibleMoves.Add(new int[] { this.Position[0] + 1, this.Position[1] - 1 });
                }
                else if (chessBoard[this.Position[0] + 1, this.Position[1] - 1].color != this.color)
                {
                    posibleMoves.Add(new int[] { this.Position[0] + 1, this.Position[1] - 1 });
                }
            }

            // diagonal down right
            if (this.Position[0] != 7 && this.Position[1] != 7)
            {
                if (chessBoard[this.Position[0] + 1, this.Position[1] + 1].GetType() == typeof(EmptyPiece))
                {
                    posibleMoves.Add(new int[] { this.Position[0] + 1, this.Position[1] + 1 });
                }
                else if (chessBoard[this.Position[0] + 1, this.Position[1] + 1].color != this.color)
                {
                    posibleMoves.Add(new int[] { this.Position[0] + 1, this.Position[1] + 1 });
                }
            }

            return Tools.CreateRectangularArray(posibleMoves);
        }
    }

}
