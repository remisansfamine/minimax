namespace TicTacToe
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			GameMgr game = new GameMgr();
            while (game.Update()) {}
		}
	}
}
