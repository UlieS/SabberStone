using System;
using SabberStoneCore.Config;
using SabberStoneCore.Enums;
using SabberStoneCoreAi.Agent;
using SabberStoneCoreAi.Agent.ExampleAgents;
using SabberStoneCoreAi.Meta;
using SabberStoneCoreAi.POGame;

namespace SabberStoneCoreAi {
	internal class Program {

		private static void Main (string[] args) {

			Console.WriteLine ("Setup gameConfig");

			//todo: rename to Main
			GameConfig gameConfig = new GameConfig {
				StartPlayer = 2,
					Player1HeroClass = CardClass.DRUID,
					Player2HeroClass = CardClass.WARLOCK,
					Player1Deck = Decks.MidrangeJadeShaman,
					Player2Deck = Decks.RenoKazakusMage,
					FillDecks = true,
					SkipMulligan = true,
			};

			Console.WriteLine ("Setup POGameHandler");
			AbstractAgent player1 = new RandomAgentLateEnd ();
			AbstractAgent player2 = new Gandalf ();
			var gameHandler = new POGameHandler (gameConfig, player1, player2, debug : true);

			Console.WriteLine ("PlayGame");
			gameHandler.PlayGame ();
			//gameHandler.PlayGames (100);
			GameStats gameStats = gameHandler.getGameStats ();

			gameStats.printResults ();

			Console.WriteLine ("Test successful");
			Console.ReadLine ();
		}
	}
}