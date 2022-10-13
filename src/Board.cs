using System;
using System.Collections.Generic;

namespace TicTacToe
{
	public class Board
	{
        Player[,] boardSquares = new Player[3, 3];
        public Player[,] BoardSquares { get { return boardSquares; } }

        Player currentPlayer = Player.Circle;
        public Player CurrentPlayer { get { return currentPlayer; } set { currentPlayer = value; } }
		public void SwitchPlayer()
		{
            if (currentPlayer == Player.Circle)
                currentPlayer = Player.Cross;
            else
                currentPlayer = Player.Circle;
        }

		public Board () {}

		public Board (Board b)
		{
            boardSquares = b.BoardSquares.Clone() as Player[,];
            currentPlayer = b.currentPlayer;
		}

		public Board Clone()
		{
			Board newBoard = new Board(this);
			return newBoard;
		}

		public void Init()
		{
			for (int i = 0; i < boardSquares.GetLength(0); i++)
			{
				for (int j = 0; j < boardSquares.GetLength(1); j++)
				{
                    boardSquares[i, j] = Player.None ;
				}
			}
		}

		public List<Move> GetAvailableMoves()
		{
			List<Move> moves = new List<Move>();
			for(int i = 0; i < boardSquares.GetLength(0); i++)
			{
				for(int j = 0; j < boardSquares.GetLength(1); j++)
				{
					if (boardSquares[i, j] == Player.None)
					{
						Move newMove = new Move();
						newMove.Line = i;
						newMove.Column = j;
						moves.Add(newMove);
					}
				}
			}
			return moves;
		}

		public void MakeMove(Move move)
		{
            if (move.Line < 0 && move.Column < 0)
            {
                Console.WriteLine("Invalid move!\n");
                return;
            }
			boardSquares[move.Line, move.Column] = currentPlayer;
			SwitchPlayer();
		}

        public void UndoMove(Move move)
        {
            if (move.Line < 0 && move.Column < 0)
            {
                Console.WriteLine("Invalid move!\n");
                return;
            }
            boardSquares[move.Line, move.Column] = Player.None;
            SwitchPlayer();
        }

        public bool IsWon()
        {
            // check lines
            for (int i = 0; i < boardSquares.GetLength(0); i++)
            {
                if (boardSquares[i, 0] != Player.None
                    && boardSquares[i, 0] == boardSquares[i, 1]
                    && boardSquares[i, 0] == boardSquares[i, 2])
                {
                    return true;
                }
            }
            // check columns
            for (int i = 0; i < boardSquares.GetLength(1); i++)
            {
                if (boardSquares[0, i] != Player.None
                    && boardSquares[0, i] == boardSquares[1, i]
                    && boardSquares[0, i] == boardSquares[2, i])
                {
                    return true;
                }
            }
            // check diagonals
            {
                if (boardSquares[1, 1] != Player.None)
                {
                    if (boardSquares[0, 0] == boardSquares[1, 1]
                        && boardSquares[0, 0] == boardSquares[2, 2])
                    {
                        return true;
                    }

                    if (boardSquares[0, 2] == boardSquares[1, 1]
                        && boardSquares[0, 2] == boardSquares[2, 0])
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private int ComputeScoreForLine(int nbSquarePlayer, int nbSquareOpponent)
        {
            int score = 0;
            if (nbSquarePlayer == 3)
                score += 100;
            else if (nbSquarePlayer == 2 && nbSquareOpponent == 0)
                score += 10;
            else if (nbSquarePlayer == 1 && nbSquareOpponent == 0)
                score += 1;
            else if (nbSquareOpponent == 3)
                score -= 100;
            else if (nbSquarePlayer == 0 && nbSquareOpponent == 2)
                score -= 10;
            else if (nbSquarePlayer == 0 && nbSquareOpponent == 1)
                score -= 1;

            return score;
        }

        private void CountSquareOwner(Player squareValue, Player player, ref int nbSquarePlayer, ref int nbSquareOpponent)
        {
            if (squareValue == player)
                nbSquarePlayer++;
            else if (squareValue != player && squareValue != Player.None)
                nbSquareOpponent++;
        }

        public int Evaluate(Player player)
		{
            int lineScore = 0;
            int score = 0;
            int nbSquarePlayer = 0, nbSquareOpponent = 0;

			// lines
			for (int i = 0; i < boardSquares.GetLength(0); i++)
			{
                nbSquarePlayer = nbSquareOpponent = 0;
                for (int j = 0; j < boardSquares.GetLength(1); j++)
                {
                    Player squareValue = boardSquares[i, j];
                    CountSquareOwner(squareValue, player, ref nbSquarePlayer, ref nbSquareOpponent);
                }
                lineScore = ComputeScoreForLine(nbSquarePlayer, nbSquareOpponent);
                // leave in case of game over
                if (Math.Abs(lineScore) == 100)
                    return lineScore;
                score += lineScore;
			}
			// columns
			for (int i = 0; i < boardSquares.GetLength(1); i++)
			{
                nbSquarePlayer = nbSquareOpponent = 0;
                for (int j = 0; j < boardSquares.GetLength(0); j++)
                {
                    Player squareValue = boardSquares[j, i];
                    CountSquareOwner(squareValue, player, ref nbSquarePlayer, ref nbSquareOpponent);
                }
                lineScore = ComputeScoreForLine(nbSquarePlayer, nbSquareOpponent);
                // leave in case of game over
                if (Math.Abs(lineScore) == 100)
                    return lineScore;
                score += lineScore;
            }
			// diagonals
            nbSquarePlayer = nbSquareOpponent = 0;
            for (int i = 0; i < boardSquares.GetLength(0); i++)
            {
                Player squareValue = boardSquares[i, i];
                CountSquareOwner(squareValue, player, ref nbSquarePlayer, ref nbSquareOpponent);
            }
            lineScore = ComputeScoreForLine(nbSquarePlayer, nbSquareOpponent);
            // leave in case of game over
            if (Math.Abs(lineScore) == 100)
                return lineScore;
            score += lineScore;

            nbSquarePlayer = nbSquareOpponent = 0;
            for (int i = 0; i < boardSquares.GetLength(0); i++)
            {
                Player squareValue = boardSquares[i, boardSquares.GetLength(1) - 1 - i];
                CountSquareOwner(squareValue, player, ref nbSquarePlayer, ref nbSquareOpponent);

            }
            lineScore = ComputeScoreForLine(nbSquarePlayer, nbSquareOpponent);

            // leave in case of game over
            if (Math.Abs(lineScore) == 100)
                return lineScore;
            score += lineScore;

            return score;
		}

        public int Evaluate()
        {
            int score = Evaluate(currentPlayer);
            return score;
        }

		public bool IsGameOver()
		{
			// check if all squares have been filled
			bool hasEmptySquare = false;
			foreach(int value in boardSquares)
			{
				if (value == (int)Player.None)
				{
					hasEmptySquare = true;
					break;
				}
			}
			if (!hasEmptySquare)
				return true;

			return IsWon();
		}

		public void Draw()
		{
			Console.Clear();
			Console.Write(" ");
			for (int j = 0; j < boardSquares.GetLength(1); j++)
				Console.Write(" {0}", j);
			Console.Write("\n -------\n");
			for (int i = 0; i < boardSquares.GetLength(0); i++)
			{
				Console.Write("{0}", i);
				for (int j = 0; j < boardSquares.GetLength(1); j++)
				{
					Console.Write("|{0}", GetSquareChar(boardSquares[i, j]));
				}
				Console.Write("|\n");
				Console.Write(" -------\n");
			}
		}

        char GetSquareChar(Player val)
		{
			switch(val)
			{
            case Player.Circle:
				return 'O';
            case Player.Cross:
				return 'X';
            case Player.None:
			default:
				return ' ';
			}
		}

	}
}

