using System;
using System.Collections.Generic;
using System.Text;
using SabberStoneCore.Enchants;
using SabberStoneCore.Enums;
using SabberStoneCore.Model;
using SabberStoneCore.Model.Entities;
using SabberStoneCore.Tasks;
using SabberStoneCore.Tasks.PlayerTasks;
using SabberStoneCoreAi.Agent;
using SabberStoneCoreAi.POGame;
using SabberStoneCore.Tasks.SimpleTasks;

namespace SabberStoneCoreAi.Agent {
	class Gandalf : AbstractAgent {
		private Random Rnd = new Random ();

		public override void FinalizeAgent () { }

		public override void FinalizeGame () { }

		public override PlayerTask GetMove (SabberStoneCoreAi.POGame.POGame poGame) {
			// gives all available options
			List<PlayerTask> options = poGame.CurrentPlayer.Options ();

			/*
				Mulligan
			*/
			// these are the cards that we want to keep			

			foreach (PlayerTask task in options) {

				if (task.PlayerTaskType == PlayerTaskType.CHOOSE) {
					IPlayable[] cardsToKeep = poGame.CurrentPlayer.HandZone.GetAll (p => p.Card.Cost < 4);
					ChooseTask chooseTask = (ChooseTask) task;

					bool equal = true;
					// dont't select any cards that we don't want in the chooseTask
					foreach (IPlayable card in cardsToKeep) {
						if (!chooseTask.Choices.Contains (card.Id)) {
							equal = false;
						}
					}
					// make sure we have the right cards
					if (equal && chooseTask.Choices.Count == cardsToKeep.Length) {
						return chooseTask;
					}
				}
			}

			//play aura minions
			PlayerTask summonTask = SummonAuraMinion (poGame);
			if (summonTask != null) {
				return summonTask;
			}

			// handle pick choose tasks
			// foreach (PlayerTask task in options) {
			// 	if (task.PlayerTaskType == PlayerTaskType.PICK) {

			// 	}
			// }


			PlayerTask chooseCardTask = ChooseCard (poGame);
			if (chooseCardTask != null){
				Console.WriteLine("Chose best card");
				//Console.WriteLine(chooseCardTask)
				return chooseCardTask;
			}

			PlayerTask coinTask = ChooseCoin (poGame);
			if (coinTask != null) {
				return coinTask;
			}

			

			PlayerTask attackTask = AttackTask (poGame);
			if (attackTask != null) {
				return attackTask;
			}

			// let the hero attack
			foreach (PlayerTask task in options) {
				if (task.PlayerTaskType == PlayerTaskType.HERO_ATTACK && task.Target == poGame.CurrentOpponent.Hero) {
					return task;
				}
			}

			// summon minions
			foreach (PlayerTask task in options) {
				if (task.PlayerTaskType == PlayerTaskType.PLAY_CARD) {
					return task;
				}
			}

			// use hero power
			foreach (PlayerTask task in options) {
				if (task.PlayerTaskType == PlayerTaskType.HERO_POWER) {
					return task;
				}
			}

			return poGame.CurrentPlayer.Options () [0];
		}

		/* 
		 * When presented with multiple Cards to pick from, choose the one with 
		 * the highest mana and rarity combined	
		 */

		private PlayerTask ChooseCard (SabberStoneCoreAi.POGame.POGame poGame) {
			foreach (PlayerTask task in poGame.CurrentPlayer.Options ()){
				if (task.PlayerTaskType == PlayerTaskType.CHOOSE) {
					int value=0;
					int bestChoice= 0;

					foreach (int entityID in task.Controller.Choice.Choices) {
						IPlayable card= task.Game.IdEntityDic[entityID];
						int newValue=card.Card.Cost+(int)card.Card.Rarity;
						
						if (value<newValue){
							bestChoice++;
							value=newValue;
						}	
					}
					return poGame.CurrentPlayer.Options ()[bestChoice];
				}
			}
			return null;
		}

		/*
		 * Determine if we should play the coin
		 */
		private PlayerTask ChooseCoin (SabberStoneCoreAi.POGame.POGame poGame) {
			foreach (PlayerTask task in poGame.CurrentPlayer.Options ()) {
				if (task.PlayerTaskType == PlayerTaskType.PLAY_CARD && task.Source.Card.Name == ("The Coin")) {
					// check if coin would add more options
					foreach (IPlayable othercards in poGame.CurrentPlayer.HandZone.GetAll ()) {
						if (othercards.Card.Cost == 1 + poGame.CurrentPlayer.BaseMana && othercards.Card.Type == CardType.MINION) {
							return task;
						}
					}
				}
			}
			return null;
		}

		/*
		 * At first we should summon minions that increase the atack of other minions	
		 */
		private PlayerTask SummonAuraMinion (SabberStoneCoreAi.POGame.POGame poGame) {
			foreach (PlayerTask task in poGame.CurrentPlayer.Options ()) {
				if (task.PlayerTaskType == PlayerTaskType.PLAY_CARD) {
					if (task.Source.GetType ().Equals (typeof (Minion))) {
						if (((Minion) task.Source).Power != null && ((Minion) task.Source).Power.Aura != null) {
							if (((Minion) task.Source).Power.Aura.Type == AuraType.ADJACENT)
								return task;
						}
					}
				}
			}
			return null;
		}

		/*
		 * Choose what should be attacked
		 */

		private PlayerTask AttackTask (SabberStoneCoreAi.POGame.POGame poGame) {
			// find taunt minions
			var tauntMinions = new List<IEntity> { };
			foreach (Minion minion in poGame.CurrentOpponent.BoardZone.GetAll ()) {
				if (minion.HasTaunt) {
					tauntMinions.Add (minion);
				}
			}
			foreach (PlayerTask task in poGame.CurrentPlayer.Options ()) {
				if (task.PlayerTaskType == PlayerTaskType.MINION_ATTACK) {
					// if we have any taunt minions attack them first
					if (tauntMinions.Contains (task.Target)) {
						return task;
					}
					Minion attacker = (Minion) task.Source;
					if (task.Target.GetType ().Equals (typeof (Minion))) {
						Minion target = (Minion) task.Target;
						// if we can attack enemy minions so that ours don't die
						// if (attacker.AttackDamage >= target.Health && target.AttackDamage < attacker.Health) {
						// 	return task;
						// }
						// if our minion has less attack we can sacrifice it by attacking
						if (attacker.AttackDamage < target.AttackDamage && attacker.AttackDamage >= target.Health) {
							return task;
						}
					}
					if (task.Target == poGame.CurrentOpponent.Hero) {
						return task;
					}
				}
			}
			return null;
		}

		public override void InitializeAgent () {
			Rnd = new Random ();
		}

		public override void InitializeGame () { }
	}
}