using System;
using System.Collections.Generic;

namespace TicTacToe
{
	public struct Move
	{
		public int Line;
		public int Column;
	}

    public enum Player
    {
        None = 0,
        Cross = 1,
        Circle = 2
    }

    public class GameMgr
    {
        bool isGameOver = false;
        public bool IsGameOver { get { return isGameOver; } }
        Board mainBoard = new Board();

        public GameMgr()
        {
            mainBoard.Init();
            mainBoard.CurrentPlayer = Player.Cross;
        }

        bool IsPlayerTurn()
        {
            return mainBoard.CurrentPlayer == Player.Cross;
        }

        private int GetPlayerInput(bool isColumn)
        {
            Console.Write("\n{0} turn : enter {1} number\n", IsPlayerTurn() ? "Player" : "Computer", isColumn ? "column" : "line");
            ConsoleKeyInfo inputKey;
            int resNum = -1;
            while (resNum < 0 || resNum > 2)
            {
                inputKey = Console.ReadKey();
                int inputNum = -1;
                if (int.TryParse(inputKey.KeyChar.ToString(), out inputNum))
                    resNum = inputNum;
            }
            return resNum;
        }

        public bool Update()
        {
            mainBoard.Draw();

            Move crtMove = new Move();
            if (IsPlayerTurn())
            {
                crtMove.Column = GetPlayerInput(true);
                crtMove.Line = GetPlayerInput(false);
                if (mainBoard.BoardSquares[crtMove.Line, crtMove.Column] == Player.None)
                {
                    mainBoard.MakeMove(crtMove);
                }
            }
            else
            {
                ComputeAIMove();
            }

            if (mainBoard.IsGameOver())
            {
                mainBoard.Draw();
                Console.Write("game over - ");
                int result = mainBoard.Evaluate(Player.Cross);
                if (result == 100)
                    Console.Write("you win\n");
                else if (result == -100)
                    Console.Write("you lose\n");
                else
                    Console.Write("it's a draw!\n");

                Console.ReadKey();

                return false;
            }
            return true;
        }
        
        // ***** AI : random move
        void ComputeAIMove()
        {
            Random rand = new Random();
            List<Move> moves = mainBoard.GetAvailableMoves();
            Move randMove = moves[rand.Next(moves.Count - 1)];
            mainBoard.MakeMove(randMove);
        }
    }
}

