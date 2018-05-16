﻿using System;
using System.Collections.Generic;
using System.Text;
using SabberStoneCore.Enchants;
using SabberStoneCore.Model.Entities;
using SabberStoneCore.Tasks;
using SabberStoneCore.Tasks.PlayerTasks;
using SabberStoneCoreAi.Agent;
using SabberStoneCoreAi.POGame;
using SabberStoneCore.Enums;
using SabberStoneCore.Model;

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

			//play adjacent aura minions
			PlayerTask summonTask = SummonAuraMinion (poGame);
			if (summonTask != null) {
				return summonTask;
			}

			// handle pick choose tasks
			// foreach (PlayerTask task in options) {
			// 	if (task.PlayerTaskType == PlayerTaskType.PICK) {

			// 	}
			// }
			
			// Console.WriteLine(poGame.CurrentPlayer.HandZone.GetAll()[0].Card.Rarity);
			// Console.WriteLine((int) Rarity.COMMON);

			PlayerTask choice=ChooseCoin(poGame);
			if (choice!=null){
				return choice;
			}

			// play by rarity
			// foreach (PlayerTask task in options){
			// 	Card highestRarity=task.Source.Card;
			// 	if (task.PlayerTaskType != PlayerTaskType.END_TURN){

			// 	}
			// 	Card highestRarity=task.Source.Card;
			// 	Console.WriteLine(Card);
				
				
			// }

			// find taunt minions
			var tauntMinions = new List<IEntity> { };
			foreach (Minion minion in poGame.CurrentOpponent.BoardZone.GetAll ()) {
				if (minion.HasTaunt) {
					tauntMinions.Add (minion);
				}
			}
			foreach (PlayerTask task in options) {
				if (task.PlayerTaskType == PlayerTaskType.MINION_ATTACK) {
					// if we have any taunt minions attack them first
					if (tauntMinions.Contains (task.Target)) {
						return task;
					}
					if (task.Target == poGame.CurrentOpponent.Hero) {
						return task;
					}
				}

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

		private PlayerTask ChooseCoin(SabberStoneCoreAi.POGame.POGame poGame) {
			foreach (PlayerTask task in poGame.CurrentPlayer.Options()) {
				if (task.PlayerTaskType == PlayerTaskType.PLAY_CARD && task.Source.Card.Name==("The Coin")){
					// check if coin would add more options
					foreach(IPlayable othercards in poGame.CurrentPlayer.HandZone.GetAll ()){
						if (othercards.Card.Cost==1+poGame.CurrentPlayer.BaseMana && othercards.Card.Type==CardType.MINION){
							Console.WriteLine("Chose Coin");
							return task;
						}
					}
				}				
			}
			return null;
		}

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

		public override void InitializeAgent () {
			Rnd = new Random ();
		}

		public override void InitializeGame () { }
	}
}