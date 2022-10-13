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

        int iterationNumber = 0;

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

            Console.WriteLine($"Iteration count: {iterationNumber}");

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

        private class MoveValuePair
        {
            public Move move = new Move();
            public int value = int.MinValue;

            public MoveValuePair() { }
            public MoveValuePair(Move move, int value) { this.move = move; this.value = value; }
            public MoveValuePair(int value) { this.move = new Move(); this.value = value; }

            public static MoveValuePair Min(in MoveValuePair a, in MoveValuePair b) => a.value < b.value ? a : b;
            public static MoveValuePair Max(in MoveValuePair a, in MoveValuePair b) => a.value > b.value ? a : b;

            public static MoveValuePair operator-(in MoveValuePair rhs) => new MoveValuePair(rhs.move, -rhs.value);
        }

        MoveValuePair MiniMax(int depth, bool isMaximizingPlayer, Move node = new Move())
        {
            if (depth == 0 || mainBoard.IsGameOver())
            {
                int heuristic = mainBoard.Evaluate(Player.Circle);
                return new MoveValuePair(node, heuristic);
            }

            iterationNumber++;

            MoveValuePair bestValue = new MoveValuePair(node, isMaximizingPlayer ? int.MinValue : int.MaxValue);

            IEnumerable<Move> moves = mainBoard.GetAvailableMoves();

            foreach (Move move in moves)
            {
                mainBoard.MakeMove(move);

                MoveValuePair currentValue = MiniMax(depth - 1, !isMaximizingPlayer, move);

                mainBoard.UndoMove(move);

                if (isMaximizingPlayer)
                {
                    if (currentValue.value > bestValue.value)
                    {
                        bestValue = currentValue;
                        bestValue.move = move;
                    }
                }
                else
                {
                    if (currentValue.value < bestValue.value)
                    {
                        bestValue = currentValue;
                        bestValue.move = move;
                    }
                }
            }

            return bestValue;
        }

        MoveValuePair MiniMaxAB(int depth, bool isMaximizingPlayer, int alpha = int.MinValue, int beta = int.MaxValue, Move node = new Move())
        {
            if (depth == 0 || mainBoard.IsGameOver())
            {
                int heuristic = mainBoard.Evaluate(Player.Circle);
                return new MoveValuePair(node, heuristic);
            }

            iterationNumber++;

            MoveValuePair bestValue = new MoveValuePair(node, isMaximizingPlayer ? int.MinValue : int.MaxValue);

            IEnumerable<Move> moves = mainBoard.GetAvailableMoves();

            foreach (Move move in moves)
            {
                mainBoard.MakeMove(move);

                MoveValuePair currentValue = MiniMaxAB(depth - 1, !isMaximizingPlayer, alpha, beta, move);

                mainBoard.UndoMove(move);

                if (isMaximizingPlayer)
                {
                    if (currentValue.value > bestValue.value)
                    {
                        bestValue = currentValue;
                        bestValue.move = move;
                    }

                    alpha = Math.Max(alpha, currentValue.value);
                }
                else
                {
                    if (currentValue.value < bestValue.value)
                    {
                        bestValue = currentValue;
                        bestValue.move = move;
                    }

                    beta = Math.Min(beta, currentValue.value);
                }

                if (beta <= alpha)
                    break;
            }

            return bestValue;
        }

        MoveValuePair NegaMax(int depth, Move node = new Move())
        {
            if (depth == 0 || mainBoard.IsGameOver())
                return new MoveValuePair(node, mainBoard.Evaluate());

            iterationNumber++;

            MoveValuePair bestValue = new MoveValuePair(node, int.MinValue);

            IEnumerable<Move> moves = mainBoard.GetAvailableMoves();

            foreach (Move move in moves)
            {
                mainBoard.MakeMove(move);

                MoveValuePair currentValue = -NegaMax(depth - 1, move);

                mainBoard.UndoMove(move);

                if (currentValue.value > bestValue.value)
                {
                    bestValue = currentValue;
                    bestValue.move = move;
                }
            }

            return bestValue;
        }

        // ***** AI : random move
        void ComputeAIMove()
        {
            iterationNumber = 0;
            MoveValuePair result = MiniMaxAB(10, true);
            //MoveValuePair result = NegaMax(10);
            //MoveValuePair result = MiniMaxAB(10, true);

            mainBoard.MakeMove(result.move);

        }
    }
}

