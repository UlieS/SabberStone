using System;
using System.Collections.Generic;
using System.Text;
using SabberStoneCore.Tasks;
using SabberStoneCoreAi.Agent;
using SabberStoneCoreAi.POGame;

namespace SabberStoneCoreAi.Agent
{
	class Gandalf : AbstractAgent
	{
		private Random Rnd = new Random();

		public override void FinalizeAgent()
		{
		}

		public override void FinalizeGame()
		{
		}

		public override PlayerTask GetMove(SabberStoneCoreAi.POGame.POGame poGame)
		{	
			// gives all available options
			List<PlayerTask> options = poGame.CurrentPlayer.Options();
			
			
			//take mulligan if needed
			foreach(PlayerTask task in options){

				if (task.PlayerTaskType == PlayerTaskType.CHOOSE){
					//throw new NotImplementedException();
					return task;
				}
			}

			// let all minions attack
			LinkedList<PlayerTask> minionAttacks = new LinkedList<PlayerTask>();
			foreach (PlayerTask task in options)
			{
				if (task.PlayerTaskType == PlayerTaskType.MINION_ATTACK && task.Target == poGame.CurrentOpponent.Hero)
				{
					minionAttacks.AddLast(task);
				}
			}
			if (minionAttacks.Count > 0)
				return minionAttacks.First.Value;

			// let the hero attack
			foreach (PlayerTask task in options){
				if(task.PlayerTaskType == PlayerTaskType.HERO_ATTACK && task.Target == poGame.CurrentOpponent.Hero){
					return task;
				}
			}

			// summon minions
			LinkedList<PlayerTask> summonMinions = new LinkedList<PlayerTask>();
			foreach (PlayerTask task in options)
			{
				if (task.PlayerTaskType == PlayerTaskType.PLAY_CARD)
				{
					summonMinions.AddLast(task);
				}
			}
			if (summonMinions.Count > 0)
				return summonMinions.First.Value;

			// use hero power
			foreach (PlayerTask task in options){
				if (task.PlayerTaskType == PlayerTaskType.HERO_POWER){
					return task;
				}
			}

			return poGame.CurrentPlayer.Options()[0];
		}

		public override void InitializeAgent()
		{
			Rnd = new Random();
		}

		public override void InitializeGame()
		{
		}
	}
}
