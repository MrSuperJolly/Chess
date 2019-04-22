using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Drawing;



namespace Chess
{


    class GameMain
    {
        static void Main(string[] args)
        {
            //Create ChessBoard and initiate game
            ChessBoard chessBoard1 = GameSystem.StartGame(8, 8);

            //Main Game loop
            while (!chessBoard1.GameOver())
            {
                // read for user inputs in the command line
                string command = Console.ReadLine();

                switch (command)
                {
                    case "white":
                        chessBoard1.DrawBoard("white");
                        break;

                    case "black":
                        chessBoard1.DrawBoard("black");
                        break;

                    case "exit":
                        Environment.Exit(0);
                        break;

                    default:
                        //checks if input was "LetterNumber LetterNumber" and a 2 valid chess coordinates
                        Regex PlayerMoveTest = new Regex(@"(^\b[A-H][1-8]\s[A-H][1-8]\b$)", RegexOptions.IgnoreCase);
                        Match PlayerMove = PlayerMoveTest.Match(command);

                        if (PlayerMove.Success)
                        {
                            //grabs the user input if valid and converts into coordinates, "A1 = 0,0" etc
                            List<int> Coordinates;
                            Coordinates = new List<int>();
                            foreach (Char x in command)
                            {

                                if (Int32.TryParse(x.ToString(), out int j))
                                {

                                    Coordinates.Add(j - 1);
                                }
                                else
                                {
                                    if (x.ToString() != " ")
                                    {
                                        int Index = Char.ToUpper(x) - 64;
                                        Coordinates.Add(Index - 1);
                                    }

                                }
                            }

                            //beautiful
                            chessBoard1.BoardData[Coordinates[0], Coordinates[1]].MovePiece(chessBoard1, new Point(Coordinates[2], Coordinates[3]), false);


                        }
                        else
                        {
                            //if nothing happens the userinput can't have been recognised
                            Console.WriteLine("Invalid Command");
                        }
                        break;

                }
            }

            chessBoard1.ChangePlayerTurn();
            Console.WriteLine(chessBoard1.PlayerTurn + " Wins!");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }


    static class GameSystem
    {


        static public ChessBoard StartGame(int BoardHeight, int BoardWidth)
        {
            //Makes a new chessboard and draws the board
            ChessBoard Board = new ChessBoard(BoardHeight, BoardWidth);
            Board.DrawBoard("white");
            return Board;
        }

        public static int GetMin(int first, int second)
        {
            //returns the smaller of the two numbers
            if (first > second)
            {
                return second;
            }
            else
            {
                return first;
            }
        }


    }






    public class ChessBoard
    {
        public readonly int BoardWidth;
        public readonly int BoardHeight;
        public Piece[,] BoardData;
        public string PlayerTurn = "white";
        public ChessBoard(int BoardWidth, int BoardHeight)
        {
            this.BoardWidth = BoardWidth;
            this.BoardHeight = BoardHeight;

            BoardData = new Piece[BoardWidth, BoardHeight];

            //temporary hard coded setup for regular chess game
            CreateRook(new Point(0, 0), "white");
            CreateRook(new Point(7, 0), "white");
            CreateRook(new Point(0, 7), "black");
            CreateRook(new Point(7, 7), "black");
            CreateKnight(new Point(1, 0), "white");
            CreateKnight(new Point(6, 0), "white");
            CreateKnight(new Point(1, 7), "black");
            CreateKnight(new Point(6, 7), "black");
            CreateBishop(new Point(2, 0), "white");
            CreateQueen(new Point(5, 0), "white");
            CreateQueen(new Point(2, 7), "black");
            CreateBishop(new Point(5, 7), "black");
            CreateQueen(new Point(3, 0), "white");
            CreateQueen(new Point(3, 7), "black");
            CreateKing(new Point(4, 0), "white");
            CreateKing(new Point(4, 7), "black");

            for (int j = 0; j < BoardHeight; j++)
            {
                for (int i = 0; i < BoardWidth; i++)
                {
                    if (j == 1)
                    {
                        CreatePawn(new Point(i, j), "white");
                    }

                    if (j == 6)
                    {
                        CreatePawn(new Point(i, j), "black");
                    }


                    if (BoardData[i, j] == null)
                    {
                        CreateEmpty(new Point(i, j));
                    }
                }
            }


        }

        public void DrawBoard(string Perspective)
        {
            Console.Clear();
            int Count = 0;
            //Creates board in ascci standard way with (0,0) at bottom left
            if (Perspective == "white")
            {

                for (int j = BoardHeight - 1; 0 <= j; j--)
                {
                    for (int i = 0; i < BoardWidth; i++)
                    {

                        Piece currentObject = BoardData[i, j];
                        if (currentObject.PieceName != "Knight")
                        {
                            Console.Write(currentObject.PieceName.Substring(0, 1) + currentObject.PieceColour.Substring(0, 1) + " ");
                        }
                        else
                        {
                            Console.Write(currentObject.PieceName.Substring(1, 1).ToUpper() + currentObject.PieceColour.Substring(0, 1) + " ");
                        }
                        Count++;

                        if (Count == BoardWidth)
                        {
                            Console.Write("\n");
                            Count = 0;
                        }

                    }
                }
            }
            //draws board in ascci in a flipped variant where (0,7) is at bottom left
            else if (Perspective == "black")
            {

                for (int j = 0; j < BoardHeight; j++)
                {
                    for (int i = 7; i >= 0; i--)
                    {

                        Piece currentObject = BoardData[i, j];
                        Console.Write(currentObject.PieceName.Substring(0, 1) + currentObject.PieceColour.Substring(0, 1) + " ");
                        Count++;


                        if (Count == BoardWidth)
                        {
                            Console.Write("\n");
                            Count = 0;
                        }

                    }
                }


            }

            Console.Write("\n" + PlayerTurn + "'s turn \n");





        }

        public bool GameOver()
        {
            Piece CurrentPiece;
            bool isValid = false;

            for (int j = 0; j < BoardHeight; j++)
            {
                for (int i = 0; i < BoardWidth; i++)
                {
                    CurrentPiece = BoardData[i, j];

                    for (int x = 0; x < BoardHeight; x++)
                    {
                        for (int y = 0; y < BoardWidth; y++)
                        {
                            isValid = CurrentPiece.MovePiece(this, new Point(x, y), true);

                            if (isValid)
                            {
                                return false;
                            }
                        }
                    }
                }

            }

            return true;
        }

        public string ChangePlayerTurn()
        {
            if (PlayerTurn == "white")
            {
                PlayerTurn = "black";
                return "black";
            }
            else
            {
                PlayerTurn = "white";
                return "white;";
            }

        }

        //functions to create piece objects literally
        private void CreateEmpty(Point pos)
        {
            BoardData[pos.X, pos.Y] = new Empty(pos);
        }

        private void CreatePawn(Point pos, string color)
        {
            BoardData[pos.X, pos.Y] = new Pawn(color, pos);
        }

        private void CreateKnight(Point pos, string color)
        {
            BoardData[pos.X, pos.Y] = new Knight(color, pos);
        }

        private void CreateBishop(Point pos, string color)
        {
            BoardData[pos.X, pos.Y] = new Bishop(color, pos);
        }

        private void CreateRook(Point pos, string color)
        {
            BoardData[pos.X, pos.Y] = new Rook(color, pos);
        }

        private void CreateQueen(Point pos, string color)
        {
            BoardData[pos.X, pos.Y] = new Queen(color, pos);
        }

        private void CreateKing(Point pos, string color)
        {
            BoardData[pos.X, pos.Y] = new King(color, pos);
        }

    }

    public abstract class Piece
    {
        public readonly string PieceName;
        public readonly string PieceColour;
        public readonly bool CanJump;
        public bool firstMove = true;
        public Point BoardPos;
        public List<Piece> PossibleMoves;
        public bool InCheck = false;

        public Piece(string PieceName, string PieceColour, Point BoardPos)
        {
            this.PieceName = PieceName;
            this.PieceColour = PieceColour;
            this.BoardPos = BoardPos;

        }


        public bool MovePiece(ChessBoard ChessBoard, Point newPosition, bool Testing)
        {
            //Attempts Moves Piece from their own position to a new user defined position
            Piece newSpace = ChessBoard.BoardData[newPosition.X, newPosition.Y];
            PossibleMoves = CheckPossibleMoves(ChessBoard);


            if (IsValidMove(PossibleMoves, newSpace) && PieceColour == ChessBoard.PlayerTurn)
            {
                Point oldPos = BoardPos;
                Piece oldPiece = ChessBoard.BoardData[BoardPos.X, BoardPos.Y];
                Piece NewPositionOldPiece = ChessBoard.BoardData[newPosition.X, newPosition.Y];
                Piece CurrentPiece;
                bool CheckException;

                CheckException = false;


                ChessBoard.BoardData[BoardPos.X, BoardPos.Y] = new Empty(BoardPos);
                BoardPos = newPosition;
                ChessBoard.BoardData[newPosition.X, newPosition.Y] = this;

                for (int j = 0; j < ChessBoard.BoardHeight; j++)
                {
                    for (int i = 0; i < ChessBoard.BoardWidth; i++)
                    {
                        CurrentPiece = ChessBoard.BoardData[i, j];
                        if (CurrentPiece.PieceName == "King")
                        {



                            if (CurrentPiece.IsInCheck(ChessBoard))
                            {
                                if (CurrentPiece.PieceColour == PieceColour)
                                {
                                    CheckException = true;
                                    break;
                                }

                            }
                        }
                    }
                }

                if (CheckException == false)
                {
                    //update board
                    if (!Testing)
                    {
                        
                        ChessBoard.ChangePlayerTurn();
                        ChessBoard.DrawBoard("white");
                        firstMove = false;
                    }
                    else
                    {
                        BoardPos = oldPos;
                        ChessBoard.BoardData[BoardPos.X, BoardPos.Y] = oldPiece;
                        ChessBoard.BoardData[newPosition.X, newPosition.Y] = NewPositionOldPiece;
                    }
                    
                    return true;
                }
                else
                {
                    BoardPos = oldPos;
                    ChessBoard.BoardData[BoardPos.X, BoardPos.Y] = oldPiece;
                    ChessBoard.BoardData[newPosition.X, newPosition.Y] = NewPositionOldPiece;
                    if (!Testing)
                    {
                        Console.WriteLine("Invalid Move King is in Check");
                    }
                    return false;
                }
            }
            else
            {
                //invalid move
                if (!Testing)
                {
                    Console.WriteLine("Invalid Move");
                }
                return false;
            }



        }


        
    
                
            
           
                            
              
          
       

        public bool IsValidMove(List<Piece> PossibleMoves, Piece newSpace)
        {

            //will check if move is in the valid move list


            if (PossibleMoves != null)
            {
                if (PossibleMoves.Any(item => item == newSpace))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool IsInCheck(ChessBoard ChessBoard)
        {
            List<Piece> ThreatSquares = new List<Piece>();

            Piece CurrentPiece;
            List<Piece> CurrentPossibleMoves;
            CurrentPossibleMoves = new List<Piece>();

            bool Check = false;

            for (int j = 0; j < ChessBoard.BoardHeight; j++)
            {
                for (int i = 0; i < ChessBoard.BoardWidth; i++)
                {
                    CurrentPiece = ChessBoard.BoardData[i, j];

                    if (CurrentPiece.PieceColour != PieceColour && CurrentPiece.PieceName != "-" && CurrentPiece.PieceName != "King")
                    {
                        CurrentPossibleMoves = CurrentPiece.CheckPossibleMoves(ChessBoard);
                        if (CurrentPossibleMoves.Any(item => item.BoardPos == BoardPos))
                        {
                            Check = true;
                            break;
                        }

                    }
                }
            }


            return Check;
        }

        public List<Piece> CheckHorizontalDirection(ChessBoard ChessBoard, Point BoardPos, bool isKing)
        {
            PossibleMoves = new List<Piece>();
            int x = BoardPos.X; 
            int y = BoardPos.Y;
            int DistanceToTop = ChessBoard.BoardHeight - y;
            int DistanceToBottom = y + 1;
            int DistanceToLeft = x + 1;
            int DistanceToRight = ChessBoard.BoardWidth - x;


            if(isKing)
            {
                DistanceToTop = GameSystem.GetMin(2, DistanceToTop);
                DistanceToBottom = GameSystem.GetMin(2, DistanceToBottom);
                DistanceToLeft = GameSystem.GetMin(2, DistanceToLeft);
                DistanceToRight = GameSystem.GetMin(2, DistanceToRight);
            }

            //check up direction

            for (int i = 1; i < DistanceToTop; i++)
            {

                Piece currentSpace = ChessBoard.BoardData[x, y + i];
                if (currentSpace.PieceColour == "-")
                {
                    PossibleMoves.Add(ChessBoard.BoardData[x, y + i]);
                }
                else if (currentSpace.PieceColour != PieceColour)
                {
                    PossibleMoves.Add(ChessBoard.BoardData[x, y + i]);
                    break;
                }
                else
                {
                    break;
                }
            }

            //check down direction

            for (int i = 1; i < DistanceToBottom; i++)
            {

                Piece currentSpace = ChessBoard.BoardData[x, y - i];
                if (currentSpace.PieceColour == "-")
                {
                    PossibleMoves.Add(ChessBoard.BoardData[x, y - i]);
                }
                else if (currentSpace.PieceColour != PieceColour)
                {
                    PossibleMoves.Add(ChessBoard.BoardData[x, y - i]);
                    break;
                }
                else
                {
                    break;
                }
            }

            //check right direction

            for (int i = 1; i < DistanceToRight; i++)
            {

                Piece currentSpace = ChessBoard.BoardData[x + i, y];
                if (currentSpace.PieceColour == "-")
                {
                    PossibleMoves.Add(ChessBoard.BoardData[x + i, y]);
                }
                else if (currentSpace.PieceColour != PieceColour)
                {
                    PossibleMoves.Add(ChessBoard.BoardData[x + i, y]);
                    break;
                }
                else
                {
                    break;
                }
            }

            //check left direction



            for (int i = 1; i < DistanceToLeft; i++)
            {

                Piece currentSpace = ChessBoard.BoardData[x - i, y];
                if (currentSpace.PieceColour == "-")
                {
                    PossibleMoves.Add(ChessBoard.BoardData[x - i, y]);
                }
                else if (currentSpace.PieceColour != PieceColour)
                {
                    PossibleMoves.Add(ChessBoard.BoardData[x - i, y]);
                    break;
                }
                else
                {
                    break;
                }
            }


            return PossibleMoves;
        }

        public List<Piece> CheckDiagonalDirection(ChessBoard ChessBoard, Point BoardPos, bool isKing)
        {
            PossibleMoves = new List<Piece>();
            int x = BoardPos.X; // 3
            int y = BoardPos.Y; // 1
            int DistanceToLeft = x + 1; // 4
            int DistanceToRight = ChessBoard.BoardWidth - x; // 5
            int DistanceToTop = ChessBoard.BoardHeight - y; // 7
            int DistanceToBottom = y + 1; //2


            if (isKing)
            {
                DistanceToTop = GameSystem.GetMin(2, DistanceToTop);
                DistanceToBottom = GameSystem.GetMin(2, DistanceToBottom);
                DistanceToLeft = GameSystem.GetMin(2, DistanceToLeft);
                DistanceToRight = GameSystem.GetMin(2, DistanceToRight);
            }

            //forward left

            for (int i = 1; i < GameSystem.GetMin(DistanceToLeft, DistanceToTop); i++)
            {
                if (i < GameSystem.GetMin(DistanceToLeft, DistanceToTop))
                {
                    Piece currentSpace = ChessBoard.BoardData[x - i, y + i];
                    if (currentSpace.PieceColour == "-")
                    {
                        PossibleMoves.Add(ChessBoard.BoardData[x - i, y + i]);
                    }
                    else if (currentSpace.PieceColour != PieceColour)
                    {
                        PossibleMoves.Add(ChessBoard.BoardData[x - i, y + i]);
                        break;
                    }
                    else
                    {
                        break;
                    }
                }


            }

            //down left 

            for (int i = 1; i < GameSystem.GetMin(DistanceToLeft, DistanceToBottom); i++)
            {




                Piece currentSpace = ChessBoard.BoardData[x - i, y - i];
                if (currentSpace.PieceColour == "-")
                {
                    PossibleMoves.Add(ChessBoard.BoardData[x - i, y - i]);
                }
                else if (currentSpace.PieceColour != PieceColour)
                {
                    PossibleMoves.Add(ChessBoard.BoardData[x - i, y - i]);
                    break;
                }
                else
                {
                    break;
                }
            }




            //forward right

            for (int i = 1; i < GameSystem.GetMin(DistanceToRight, DistanceToTop); i++)
            {


                Piece currentSpace = ChessBoard.BoardData[x + i, y + i];
                if (currentSpace.PieceColour == "-")
                {
                    PossibleMoves.Add(ChessBoard.BoardData[x + i, y + i]);
                }
                else if (currentSpace.PieceColour != PieceColour)
                {
                    PossibleMoves.Add(ChessBoard.BoardData[x + i, y + i]);
                    break;
                }
                else
                {
                    break;
                }


            }

            //down right

            for (int i = 1; i < GameSystem.GetMin(DistanceToRight, DistanceToBottom); i++)
            {


                Piece currentSpace = ChessBoard.BoardData[x + i, y - i];
                if (currentSpace.PieceColour == "-")
                {
                    PossibleMoves.Add(ChessBoard.BoardData[x + i, y - i]);
                }
                else if (currentSpace.PieceColour != PieceColour)
                {
                    PossibleMoves.Add(ChessBoard.BoardData[x + i, y - i]);
                    break;
                }
                else
                {
                    break;
                }



            }

            return PossibleMoves;
        }

        public abstract List<Piece> CheckPossibleMoves(ChessBoard ChessBoard);
    }


    public class Pawn : Piece
    {



        public Pawn(string PieceColour, Point BoardPos) : base("Pawn", PieceColour, BoardPos)
        {
        }



        public override List<Piece> CheckPossibleMoves(ChessBoard ChessBoard)
        {


            PossibleMoves = new List<Piece>();
            Piece Test1;
            Piece Test2;
            Piece Test3;
            Piece Test4;
            int x = BoardPos.X;
            int y = BoardPos.Y;

            //Pawn validation, returns a list of possible moves for this object based on it's location on the board and opther pieces
            if (PieceColour == "white")
            {
                //what's spaces to check
                if (y + 1 > ChessBoard.BoardHeight - 1)
                {
                    return PossibleMoves;
                }
                else
                {
                    Test1 = ChessBoard.BoardData[x, y + 1];

                    if (x + 1 > ChessBoard.BoardWidth - 1)
                    {
                        Test2 = null;
                    }
                    else
                    {
                        Test2 = ChessBoard.BoardData[x + 1, y + 1];
                    }

                    if (x - 1 < 0)
                    {
                        Test3 = null;
                    }
                    else
                    {
                        Test3 = ChessBoard.BoardData[x - 1, y + 1];
                    }
                }



                if (firstMove == true && y + 2 < ChessBoard.BoardHeight - 1)
                {
                    Test4 = ChessBoard.BoardData[x, y + 2];
                    
                }
                else
                {
                    Test4 = null;
                }

                if (Test1 != null)
                {
                    if (Test1.PieceName == "-")
                    {
                        PossibleMoves.Add(Test1);
                    }
                }

                if (Test2 != null)
                {
                    if (Test2.PieceName != "-" && Test2.PieceColour == "black")
                    {
                        PossibleMoves.Add(Test2);
                    }
                }

                if (Test3 != null)
                {
                    if (Test3.PieceName != "-" && Test3.PieceColour == "black")
                    {
                        PossibleMoves.Add(Test3);
                    }
                }


                if (Test4 != null)
                {
                    if (Test4.PieceName == "-")
                    {
                        PossibleMoves.Add(Test4);
                    }
                }
            }

            if (PieceColour == "black")
            {
                //what's spaces to check


                if (y - 1 < 0)
                {
                    return PossibleMoves;
                }
                else
                {
                    Test1 = ChessBoard.BoardData[x, y - 1];
                    if (Test1.PieceName == "-")
                    {
                        PossibleMoves.Add(Test1);
                    }


                    if (x - 1 < 0)
                    {
                        Test2 = null;
                    }
                    else
                    {
                        Test2 = ChessBoard.BoardData[x - 1, y - 1];

                        if (Test2.PieceName != "-" && Test2.PieceColour == "white")
                        {
                            PossibleMoves.Add(Test2);
                        }
                    }

                    if (x + 1 > ChessBoard.BoardWidth - 1)
                    {
                        Test3 = null;
                    }
                    else
                    {
                        Test3 = ChessBoard.BoardData[x + 1, y - 1];

                        if (Test3.PieceName != "-" && Test3.PieceColour == "white")
                        {
                            PossibleMoves.Add(Test3);
                        }
                    }
                }



                if (firstMove == true && y - 2 > 0)
                {
                    Test4 = ChessBoard.BoardData[x, y - 2];
                }
                else
                {
                    Test4 = null;
                }







                if (Test4 != null)
                {
                    if (Test4.PieceName == "-")
                    {
                        PossibleMoves.Add(Test4);
                    }
                }


            }


            return PossibleMoves;

        }

    }

    public class Queen : Piece
    {
            public Queen(string PieceColour, Point BoardPos) : base("Queen", PieceColour, BoardPos)
            {
            }



            public override List<Piece> CheckPossibleMoves(ChessBoard ChessBoard)
            {
                PossibleMoves = new List<Piece>();
                PossibleMoves = CheckHorizontalDirection(ChessBoard, BoardPos, false).Concat(CheckDiagonalDirection(ChessBoard, BoardPos, false)).ToList();
                return PossibleMoves;



            }
    }


        public class Rook : Piece
        {
            public Rook(string PieceColour, Point BoardPos) : base("Rook", PieceColour, BoardPos)
            {
            }



            public override List<Piece> CheckPossibleMoves(ChessBoard ChessBoard)
            {

                return CheckHorizontalDirection(ChessBoard, BoardPos, false);

            }
        }

            public class Bishop : Piece
            {
                public Bishop(string PieceColour, Point BoardPos) : base("Bishop", PieceColour, BoardPos)
                {
                }



                public override List<Piece> CheckPossibleMoves(ChessBoard ChessBoard)
                {
                   return  CheckDiagonalDirection(ChessBoard, BoardPos, false);
                }
            }

            public class Knight : Piece
            {
                public Knight(string PieceColour, Point BoardPos) : base("Knight", PieceColour, BoardPos)
                {
                }



                public override List<Piece> CheckPossibleMoves(ChessBoard ChessBoard)
                {
                    PossibleMoves = new List<Piece>();
                    int x = BoardPos.X; 
                    int y = BoardPos.Y; 
                    int DistanceToLeft = x + 1; 
                    int DistanceToRight = ChessBoard.BoardWidth - x; 
                    int DistanceToTop = ChessBoard.BoardHeight - y; 
                    int DistanceToBottom = y + 1; 
                    
                    if(DistanceToLeft >= 3 && DistanceToBottom >= 2 && PieceColour != ChessBoard.BoardData[x - 2, y - 1].PieceColour)
                    {
                        PossibleMoves.Add(ChessBoard.BoardData[x - 2, y - 1]);
                    }

                    if (DistanceToLeft >= 3 && DistanceToTop >= 2 && PieceColour != ChessBoard.BoardData[x - 2, y + 1].PieceColour)
                    {
                        PossibleMoves.Add(ChessBoard.BoardData[x - 2, y + 1]);
                    }

                    if (DistanceToRight >= 3 && DistanceToBottom >= 2 && PieceColour != ChessBoard.BoardData[x + 2, y - 1].PieceColour)
                    {
                        PossibleMoves.Add(ChessBoard.BoardData[x + 2, y - 1]);
                    }

                    if (DistanceToRight >= 3 && DistanceToTop >= 2 && PieceColour != ChessBoard.BoardData[x + 2, y + 1].PieceColour)
                    {
                        PossibleMoves.Add(ChessBoard.BoardData[x + 2, y + 1]);
                    }

                    if (DistanceToTop >= 3 && DistanceToLeft >= 2 && PieceColour != ChessBoard.BoardData[x -1, y + 2].PieceColour)
                    {
                        PossibleMoves.Add(ChessBoard.BoardData[x - 1, y + 2]);
                    }

                    if (DistanceToTop >= 3 && DistanceToRight >= 2 && PieceColour != ChessBoard.BoardData[x + 1, y + 2].PieceColour)
                    {
                        PossibleMoves.Add(ChessBoard.BoardData[x + 1, y + 2]);
                    }

                    if (DistanceToBottom >= 3 && DistanceToLeft >= 2 && PieceColour != ChessBoard.BoardData[x - 1, y - 2].PieceColour)
                    {
                        PossibleMoves.Add(ChessBoard.BoardData[x - 1, y - 2]);
                    }

                    if (DistanceToBottom >= 3 && DistanceToRight >= 2 && PieceColour != ChessBoard.BoardData[x + 1, y - 2].PieceColour)
                    {
                        PossibleMoves.Add(ChessBoard.BoardData[x + 1, y - 2]);
                    }





                   return PossibleMoves;

                }
            }



            public class King : Piece
            {
               
                
                public King(string PieceColour, Point BoardPos) : base("King", PieceColour, BoardPos)
                {
                }

                

                public override List<Piece> CheckPossibleMoves(ChessBoard ChessBoard)
                {

                    PossibleMoves = new List<Piece>();
                    PossibleMoves = CheckHorizontalDirection(ChessBoard, BoardPos, true).Concat(CheckDiagonalDirection(ChessBoard, BoardPos, true)).ToList();
                    return PossibleMoves;

            
                }
            }


            public class Empty : Piece
            {
                public Empty(Point BoardPos) : base("-", "-", BoardPos)
                {
                }

                public override List<Piece> CheckPossibleMoves(ChessBoard ChessBoard)
                {
                    //will return a list of valid moves
                    return new List<Piece>();
                }
            }
}
    

