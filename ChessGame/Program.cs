using System.Diagnostics;
using System.Numerics;
using System.Xml;

namespace ChessGame
{
    internal class Program
    {
        // TODO LIST:
        // CHECKMATE DETECTION
        // CASTLING

        static void Main(string[] args)
        {
            Console.Title = "Chess Game";

            Console.Write("Press any key to begin ... ");
            Console.ReadKey();
            Console.WriteLine();

            bool isProgramOnGoing = true;

            // This is the App loop
            while (isProgramOnGoing) 
            {
                Piece[,] chessBoard = FillChessBoard();

                int turn = 0;
                string player = "white";
                bool isGameOnGoing = true;

                Console.WriteLine("New game started");

                // This is the loop for each chess game, it ends when the chess game ends
                while (isGameOnGoing)
                {
                    //Print info
                    PrintChessBoard(chessBoard);
                    Console.WriteLine("================================");
                    Console.WriteLine($"{player}'s turn \t | \t turn: {turn}");
                    Console.WriteLine("================================");
                    Console.WriteLine();
                    Console.Write("Write a command (help, move, restart, exit) : ");

                    // Get user action 
                    string command = Console.ReadLine();
                    switch (command)
                    {
                        case "exit":
                            isGameOnGoing = false;
                            isProgramOnGoing = false;
                            break;
                        case "move":
                            //If MoveCommand returns true, it is checkmate, else continue
                            if (MoveCommand(ref chessBoard, player))
                            {
                                Console.Clear();
                                //Show last move
                                PrintChessBoard(chessBoard);
                                Console.WriteLine("================================");
                                Console.WriteLine($"{player}'s turn \t | \t turn: {turn}");
                                Console.WriteLine("================================");
                                Console.WriteLine();

                                Console.WriteLine($"Checkmate, {player} won!");
                                Console.WriteLine("Press any key to restart ...");
                                Console.ReadKey();
                                isGameOnGoing = false;
                                break;
                            }
                            else
                            {
                                turn++;
                                player = turn % 2 == 0 ? player = "white" : player = "black";
                                break;
                            }
                        case "restart":
                            isGameOnGoing = false;
                            break;
                        case "help":
                            Help();
                            Console.WriteLine("Press any key to continue ... ");
                            Console.ReadKey();
                            break;
                        default:
                            break;
                    }

                    Console.Clear();
                }

                Console.WriteLine("Game ended");

            }
            Console.WriteLine("Exiting app");
        }
        static public bool MoveCommand(ref Piece[,] chessBoard, string player)
        {
            ///<summary>
            /// 1.- Asks player for the desired move
            /// 2.- If the move is posible it does it
            /// 3.- Checks if there is check mate
            /// 4.- returns true if game has ended, false if game continues
            ///</summary>

            int[] selSquare = new int[] { -1, -1 };
            int[] newSquare = new int[] { -1, -1 };
            bool isMoveCorrect;
            do
            {
                // Get move command
                isMoveCorrect = true;
                Console.Write("Write: [Piece]-[New Position] : ");
                string move = Console.ReadLine();

                // --------------- TESTS TO CHECK IF INPUT IS CORRECT ---------------

                // Check if format is correct (if it doesn't contain "-" we know it's wrong)
                if (!move.Contains("-") || move == null || move.Length != 5)
                {
                    isMoveCorrect = false;
                    Console.WriteLine("error: Incorrect format");
                }

                // Now check if we can parse the input, if so parse it and store it
                if (isMoveCorrect)
                {
                    if (!int.TryParse(move.Substring(0, 2), out int n) || (!int.TryParse(move.Substring(3, 2), out int nu)))
                    {
                        isMoveCorrect = false;
                        Console.WriteLine("error: Incorrect format");
                    }
                    else
                    {
                        selSquare = new int[] { Convert.ToInt16(move.Substring(0, 1)), Convert.ToInt16(move.Substring(1, 1)) };
                        newSquare = new int[] { Convert.ToInt16(move.Substring(3, 1)), Convert.ToInt16(move.Substring(4, 1)) };
                    }

                    // Check if the squares selected are between 0 and 8, if not it is wrong
                    foreach (int num in selSquare)
                    {
                        if (num <= -1 || num >= 8)
                        {
                            isMoveCorrect = false;
                            Console.WriteLine($"error: There is no coordinate '{num}' in a chessboard");
                        }
                    }
                    foreach (int num in newSquare)
                    {
                        if (num <= -1 || num >= 8)
                        {
                            isMoveCorrect = false;
                            Console.WriteLine($"error: There is no coordinate '{num}' in a chessboard");
                        }
                    }
                }

                // Check if the pieces selected can do what is commanded
                if (isMoveCorrect)
                {
                    //Get the pieces
                    Piece pieceChosen = chessBoard[selSquare[0], selSquare[1]];
                    Piece pieceEaten = chessBoard[newSquare[0], newSquare[1]];

                    // Check if the piece chosen isn't a piece
                    if (pieceChosen.GetType() == typeof(EmptyPiece))
                    {
                        isMoveCorrect = false;
                        Console.WriteLine("error: You have no piece in the selected square");
                    }

                    // Check if you chose an oponents piece
                    if (pieceChosen.color != player)
                    {
                        isMoveCorrect = false;
                        Console.WriteLine("error: You can't move your opponents piece");
                    }

                    // Check if you are eating your own piece
                    if (pieceEaten.color == player && pieceEaten.GetType() != typeof(EmptyPiece))
                    {
                        isMoveCorrect = false;
                        Console.WriteLine("error: You can't eat your own piece");
                    }

                    // Check if the move desired is within the piece's moves
                    if (isMoveCorrect)
                    {
                        int[,] pieceMoves = pieceChosen.GetPosibleMoves(chessBoard);

                        isMoveCorrect = false;
                        for (int i = 0; i < pieceMoves.GetLength(0); i++)
                        {
                            if (pieceMoves[i, 0] == newSquare[0] && pieceMoves[i, 1] == newSquare[1] && pieceMoves[0, 0] != -1)
                            {
                                isMoveCorrect = true;
                            }
                        }
                        if (!isMoveCorrect) Console.WriteLine("error: That piece can't move there");
                    }

                    // Check if the move would cause your king to be under check
                    if (isMoveCorrect)
                    {
                        isMoveCorrect = Piece.IsUnderCheck(chessBoard, selSquare, newSquare, player) == false ? true : false;
                    }

                    // ------------------ END OF TESTS FOR INPUT --------------------

                    // If every test is correct the move is done
                    if (isMoveCorrect)
                    {
                        chessBoard[newSquare[0], newSquare[1]] = pieceChosen;
                        string colorEmptySquare = (selSquare[0] + selSquare[1]) % 2 == 0 ? "white" : "black";
                        chessBoard[selSquare[0], selSquare[1]] =
                            new EmptyPiece(chessBoard[selSquare[0], selSquare[1]].GetPosition(), colorEmptySquare);
                        pieceChosen.SetPosition(newSquare);

                        // If the move is a pawn and it is on the last row then it promotes
                        if (pieceChosen.GetType() == typeof(Pawn) && (pieceChosen.GetPosition()[0] == 0 || pieceChosen.GetPosition()[0] == 7)) // TO PROMOTE PAWNS
                        {
                            bool isInputValid = false;
                            do
                            {
                                // Get input of desired promotion
                                Console.WriteLine($"Your pawn in {pieceChosen.GetPosition()[0]}{pieceChosen.GetPosition()[1]} has reached the last row!");
                                Console.WriteLine($"Choose the piece you want to promote to (horse, bishop, rook, queen) :");
                                string input = Console.ReadLine();

                                // Sustitute the piece with the promotion
                                switch (input)
                                {
                                    case "horse":
                                        chessBoard[newSquare[0], newSquare[1]] = new Horse(new int[] { newSquare[0], newSquare[1] }, pieceChosen.color);
                                        Console.WriteLine("Pawn promoted to horse");
                                        isInputValid = true;
                                        break;
                                    case "bishop":
                                        chessBoard[newSquare[0], newSquare[1]] = new Bishop(new int[] { newSquare[0], newSquare[1] }, pieceChosen.color);
                                        Console.WriteLine("Pawn promoted to bishop");
                                        isInputValid = true;
                                        break;
                                    case "rook":
                                        chessBoard[newSquare[0], newSquare[1]] = new Rook(new int[] { newSquare[0], newSquare[1] }, pieceChosen.color);
                                        Console.WriteLine("Pawn promoted to rook");
                                        isInputValid = true;
                                        break;
                                    case "queen":
                                        chessBoard[newSquare[0], newSquare[1]] = new Queen(new int[] { newSquare[0], newSquare[1] }, pieceChosen.color);
                                        Console.WriteLine("Pawn promoted to queen");
                                        isInputValid = true;
                                        break;

                                    // if input is incorrect ask again
                                    default:
                                        Console.WriteLine("error: You can't promote to that");
                                        break;

                                }
                            } while (!isInputValid);
                        }

                        //Check if the move causes checkmate
                        if (IsCheckMate(chessBoard, player == "white" ? "black" : "white")) return true;
                    }
                }
            } while (!isMoveCorrect);

            // If it reaches here then it wasn't checkmate
            return false;
        }
        static public Piece[,] FillChessBoard()
        {
            ///<summary>
            /// Just fills the board with every piece so you don't have to
            ///</summary>
            

            Piece[,] chessBoard = new Piece[8, 8];
            chessBoard[0, 0] = new Rook(new int[] { 0, 0 }, "black");
            chessBoard[0, 1] = new Horse(new int[] { 0, 1 }, "black");
            chessBoard[0, 2] = new Bishop(new int[] { 0, 2 }, "black");
            chessBoard[0, 3] = new Queen(new int[] { 0, 3 }, "black");
            chessBoard[0, 4] = new King(new int[] { 0, 4 }, "black");
            chessBoard[0, 5] = new Bishop(new int[] { 0, 5 }, "black");
            chessBoard[0, 6] = new Horse(new int[] { 0, 6 }, "black");
            chessBoard[0, 7] = new Rook(new int[] { 0, 7 }, "black");

            chessBoard[7, 0] = new Rook(new int[] { 7, 0 }, "white");
            chessBoard[7, 1] = new Horse(new int[] { 7, 1 }, "white");
            chessBoard[7, 2] = new Bishop(new int[] { 7, 2 }, "white");
            chessBoard[7, 3] = new Queen(new int[] { 7, 3 }, "white");
            chessBoard[7, 4] = new King(new int[] { 7, 4 }, "white");
            chessBoard[7, 5] = new Bishop(new int[] { 7, 5 }, "white");
            chessBoard[7, 6] = new Horse(new int[] { 7, 6 }, "white");
            chessBoard[7, 7] = new Rook(new int[] { 7, 7 }, "white");

            // The pawns and empty pieces are set with a loop because im lazy
            for (int i = 0; i < 8; i++)
            {
                chessBoard[1, i] = new Pawn(new int[] { 1, i }, "black");
                chessBoard[6, i] = new Pawn(new int[] { 6, i }, "white");
            }
            for (int i = 2; i < 6; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    chessBoard[i, j] = (i + j) % 2 == 0 ?
                        new EmptyPiece(new int[] { i, j }, "white") : new EmptyPiece(new int[] { i, j }, "black");
                }
            }

            return chessBoard;
        }

        static public void PrintChessBoard(Piece[,] chessBoard)
        {
            ///<summary>
            /// Takes a chessboard and writes it on the console with the decorations :)
            /// </summary>

            for(int i = 0; i < 11; i++)
            {
                if ((i >= 0 && i <= 1) || i == 10)
                {
                    Console.Write("    ");
                }
                else 
                { 
                    Console.Write(i - 2);
                    Console.Write(" | ");
                }

                for (int j = 0; j < 9; j++)
                {
                    if (i == 0 && j<=7) Console.Write(j + "  ");

                    if ((i == 1 || i == 10) && j <= 7) Console.Write("---");

                    if (i != 0 && i != 1 && i != 10 && j != 8) 
                    { 
                        Console.Write(chessBoard[i-2, j].icon + " "); 
                    }

                    if(j == 8 && i >= 2 && i <= 9) Console.Write("|");


                }
                Console.WriteLine();
            
            }
            Console.WriteLine();

        }
        static public void Help()
        {
            Console.WriteLine("Commands: ");
            Console.WriteLine(" - move: Allows you to make a move");
            Console.WriteLine("\t* After enterign \"move\", you'll be asked to type in the move you want to make.\n\t" +
                                        "The format to move a piece is [coords of your piece]-[coords of where you want to move]\n\t" +
                                        "The coords have to be written as a two digit number, the first digit being the rows and the\n\t" +
                                        "second number being the columns -> e.g. 23-24 (moves the piece one square to the right)");
            Console.WriteLine(" - restart: Restarts the chessboard");
            Console.WriteLine(" - exit: Exits the game");
            Console.WriteLine();
        }

        static public bool IsCheckMate(Piece[,] chessBoard, string player)
        {
            int[] kingSquare = null;

            //Finds the player king
            foreach (Piece piece in chessBoard) 
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

            //Sees if an enemy piece threatens the king
            foreach (Piece piece in chessBoard)
            {
                //skips ally piece and empty squares
                if (piece.color == player || piece.GetType() == typeof(EmptyPiece)) 
                {
                    continue;
                }

                int[,] pieceMoves = piece.GetPosibleMoves(chessBoard);

                //sees if a move from said piece can take the king
                for (int i = 0; i < pieceMoves.GetLength(0); i++) 
                {
                    //Checks if the move takes the king
                    if (pieceMoves[i, 0] == kingSquare[0] && pieceMoves[i, 1] == kingSquare[1])
                    {
                        //If a move from that piece can take the king checks if any ally piece can defend
                        foreach(Piece defendingPiece in chessBoard)
                        {
                            if (defendingPiece.color != player || defendingPiece.GetType() == typeof(EmptyPiece)) //skips ally piece and empty squares
                            {
                                continue;
                            }

                            int[,] defendingPieceMoves = defendingPiece.GetPosibleMoves(chessBoard);

                            //Sees if after any of the posible moves the king is not checked
                            for (int j = 0; j < defendingPieceMoves.GetLength(0); j++)
                            {
                                //If a piece has a move which avoids mate then it isn't checkmate
                                if(!Piece.IsUnderCheck(chessBoard, defendingPiece.GetPosition(), new[] { defendingPieceMoves[j, 0], defendingPieceMoves[j, 1] }, player)) 
                                    return false;
                            }
                        }

                        //if no defending piece was found then it's checkmate
                        return true;
                    }
                }

            }
            // this means the king wasn't under check because no enemy piece can take the king
            return false;
        }
    }
}