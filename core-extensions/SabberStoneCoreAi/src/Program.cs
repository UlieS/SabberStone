using System;
using SabberStoneCore.Config;
using SabberStoneCore.Enums;
using SabberStoneCoreAi.Agent;
using SabberStoneCoreAi.Agent.ExampleAgents;
using SabberStoneCoreAi.POGame;

namespace SabberStoneCoreAi {
	internal class Program {

		private static void Main (string[] args) {

			Console.WriteLine ("Setup gameConfig");

			//todo: rename to Main
			GameConfig gameConfig = new GameConfig {
				StartPlayer = 2,
					Player1HeroClass = CardClass.DRUID,
					Player2HeroClass = CardClass.MAGE,
					FillDecks = true,
					SkipMulligan = false,
			};

			Console.WriteLine ("Setup POGameHandler");
			AbstractAgent player1 = new Gandalf ();
			AbstractAgent player2 = new FaceHunter ();
			var gameHandler = new POGameHandler (gameConfig, player1, player2, debug : true);

			Console.WriteLine ("PlayGame");
			gameHandler.PlayGame ();
			//gameHandler.PlayGames(10);
			GameStats gameStats = gameHandler.getGameStats ();

			gameStats.printResults ();

			Console.WriteLine ("Test successful");
			Console.ReadLine ();
		}
	}
}