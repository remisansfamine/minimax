using System;
using System.Collections.Generic;

namespace TicTacToe
{
    public enum EAlgorithm
    {
        MINIMAX,
        NEGAMAX,
        MINIMAXAB,
        NEGAMAXAB,
    }

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
        int depth = 10;

        EAlgorithm algorithm = EAlgorithm.MINIMAX;

        bool isGameOver = false;
        public bool IsGameOver { get { return isGameOver; } }
        Board mainBoard = new Board();

        int iterationNumber = 0;

        public GameMgr()
        {
            mainBoard.Init();
            mainBoard.CurrentPlayer = Player.Cross;

            int algoType = GetAlgorithmType();

            algorithm = (EAlgorithm)algoType;
        }

        bool IsPlayerTurn()
        {
            return mainBoard.CurrentPlayer == Player.Cross;
        }

        private int GetAlgorithmType()
        {
            Console.WriteLine("Which algorithm do you want to use ?\n1. MiniMax\n2. NegaMax\n3. MiniMaxAB\n4. NegaMaxAB");
            ConsoleKeyInfo inputKey;
            int resNum = -1;
            while (resNum < 0 || resNum > 3)
            {
                inputKey = Console.ReadKey();
                if (int.TryParse(inputKey.KeyChar.ToString(), out int inputNum))
                    resNum = inputNum - 1;
            }
            return resNum;
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

            Console.WriteLine("Current algorithm: {0}", algorithm.ToString());
            Console.WriteLine("Iteration count: {0}", iterationNumber);

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

        private struct MoveValuePair
        {
            public Move move;
            public int value;

            public MoveValuePair(Move move, int value) { this.move = move; this.value = value; }
            public MoveValuePair(int value) { this.move = new Move(); this.value = value; }
        }

        MoveValuePair MiniMax(int depth, bool isMaximizingPlayer, Move node = new Move())
        {
            // Check if this is the final node, return only the heuristic 
            if (depth == 0 || mainBoard.IsGameOver())
            {
                int heuristic = mainBoard.Evaluate(Player.Circle);
                return new MoveValuePair(heuristic);
            }

            iterationNumber++;

            MoveValuePair bestValue = new MoveValuePair(node, isMaximizingPlayer ? int.MinValue : int.MaxValue);

            IEnumerable<Move> moves = mainBoard.GetAvailableMoves();

            foreach (Move move in moves)
            {
                int currentHeuristic;
                {
                    // Make a move and undo it in the same board to avoid useless cloning
                    mainBoard.MakeMove(move);

                    currentHeuristic = MiniMax(depth - 1, !isMaximizingPlayer, move).value;

                    mainBoard.UndoMove(move);
                }

                if (isMaximizingPlayer)
                {
                    if (currentHeuristic > bestValue.value)
                        bestValue = new MoveValuePair(move, currentHeuristic);
                }
                else
                {
                    if (currentHeuristic < bestValue.value)
                        bestValue = new MoveValuePair(move, currentHeuristic);
                }
            }

            return bestValue;
        }

        MoveValuePair MiniMaxAB(int depth, bool isMaximizingPlayer, int alpha = int.MinValue, int beta = int.MaxValue, Move node = new Move())
        {
            // Check if this is the final node, return only the heuristic 
            if (depth == 0 || mainBoard.IsGameOver())
            {
                int heuristic = mainBoard.Evaluate(Player.Circle);
                return new MoveValuePair(heuristic);
            }

            iterationNumber++;

            MoveValuePair bestValue = new MoveValuePair(node, isMaximizingPlayer ? int.MinValue : int.MaxValue);

            IEnumerable<Move> moves = mainBoard.GetAvailableMoves();

            foreach (Move move in moves)
            {
                int currentHeuristic;
                {
                    // Make a move and undo it in the same board to avoid useless cloning
                    mainBoard.MakeMove(move);

                    currentHeuristic = MiniMaxAB(depth - 1, !isMaximizingPlayer, alpha, beta, move).value;

                    mainBoard.UndoMove(move);
                }

                if (isMaximizingPlayer)
                {
                    if (currentHeuristic > bestValue.value)
                        bestValue = new MoveValuePair(move, currentHeuristic);

                    alpha = Math.Max(alpha, currentHeuristic);
                }
                else
                {
                    if (currentHeuristic < bestValue.value)
                        bestValue = new MoveValuePair(move, currentHeuristic);

                    beta = Math.Min(beta, currentHeuristic);
                }

                // Cut-off
                if (beta <= alpha)
                    break;
            }

            return bestValue;
        }

        MoveValuePair NegaMax(int depth, Move node = new Move())
        {
            // Check if this is the final node, return only the heuristic 
            if (depth == 0 || mainBoard.IsGameOver())
            {
                int heuristic = mainBoard.Evaluate();
                return new MoveValuePair(heuristic);
            }

            iterationNumber++;

            MoveValuePair bestValue = new MoveValuePair(node, int.MinValue);

            IEnumerable<Move> moves = mainBoard.GetAvailableMoves();

            foreach (Move move in moves)
            {
                int currentHeuristic;
                {
                    // Make a move and undo it in the same board to avoid useless cloning
                    mainBoard.MakeMove(move);

                    currentHeuristic = -NegaMax(depth - 1, move).value;

                    mainBoard.UndoMove(move);
                }

                if (currentHeuristic > bestValue.value)
                    bestValue = new MoveValuePair(move, currentHeuristic);
            }

            return bestValue;
        }

        MoveValuePair NegaMaxAB(int depth, int alpha, int beta, Move node = new Move())
        {
            // Check if this is the final node, return only the heuristic 
            if (depth == 0 || mainBoard.IsGameOver())
            {
                int heuristic = mainBoard.Evaluate();
                return new MoveValuePair(heuristic);
            }

            iterationNumber++;

            MoveValuePair bestValue = new MoveValuePair(node, int.MinValue);

            IEnumerable<Move> moves = mainBoard.GetAvailableMoves();

            foreach (Move move in moves)
            {
                int currentHeuristic;
                {
                    // Make a move and undo it in the same board to avoid useless cloning
                    mainBoard.MakeMove(move);

                    currentHeuristic = -NegaMaxAB(depth - 1, -beta, -alpha, move).value;

                    mainBoard.UndoMove(move);
                }

                if (currentHeuristic > bestValue.value)
                    bestValue = new MoveValuePair(move, currentHeuristic);

                alpha = Math.Max(alpha, currentHeuristic);

                // Cut-off
                if (alpha >= beta)
                    break;
            }

            return bestValue;
        }

        // ***** AI : random move
        void ComputeAIMove()
        {
            iterationNumber = 0;

            MoveValuePair result;
            switch (algorithm)
            {
                case EAlgorithm.MINIMAX:
                default:
                    result = MiniMax(depth, true);
                    break;

                case EAlgorithm.MINIMAXAB:
                    result = MiniMaxAB(depth, true);
                    break;

                case EAlgorithm.NEGAMAX:
                    result = NegaMax(depth);
                    break;

                case EAlgorithm.NEGAMAXAB:
                    result = NegaMaxAB(depth, -1000, 1000);
                    break;
            }

            mainBoard.MakeMove(result.move);
        }
    }
}

