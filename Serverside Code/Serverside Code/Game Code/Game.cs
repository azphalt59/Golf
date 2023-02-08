using System;
using System.Collections.Generic;
using PlayerIO.GameLibrary;

namespace GoldDLL {
	public class Player : BasePlayer {
		public float posx = 0;
		public float posz = 0;
		public float posy= 0;
	}
	
	

	[RoomType("Golf")]
	public class GameCode : Game<Player> {
	
		public int playerIndex = 0;
		private Dictionary<string, float[]> playerBalls = new Dictionary<string, float[]>();


		// This method is called when an instance of your the game is created
		public override void GameStarted() {
			// anything you write to the Console will show up in the 
			// output window of the development server
			Console.WriteLine("Game is started: " + RoomId);

		}

		

		// This method is called when the last player leaves the room, and it's closed down.
		public override void GameClosed() {
			Console.WriteLine("RoomId: " + RoomId);
		}

		// This method is called whenever a player joins the game
		public override void UserJoined(Player player)
		{

			if(!playerBalls.ContainsKey(player.ConnectUserId))
            {
				playerBalls.Add(player.ConnectUserId, new float[] { 0, 0, 0 });
            }
			else
            {
				playerBalls[player.ConnectUserId] = new float[] { 0, 0, 0 };
			}
			
			foreach (Player pl in Players) {
				if(pl.ConnectUserId != player.ConnectUserId) {
					pl.Send("PlayerJoined", player.ConnectUserId, 0,0, 0);
					player.Send("PlayerJoined", pl.ConnectUserId, pl.posx, pl.posy, pl.posz);
				}
			}

		}

		// This method is called when a player leaves the game
		public override void UserLeft(Player player) {
			Broadcast("PlayerLeft", player.ConnectUserId);
		}

		// This method is called when a player sends a message into the server code
		public override void GotMessage(Player player, Message message) {
			switch(message.Type) {
				// called when a player clicks on the ground
				case "BallMove":
					string playerId = message.GetString(0);
					float x = message.GetFloat(1);
					float y = message.GetFloat(2);
					float z = message.GetFloat(3);
					if (!playerBalls.ContainsKey(player.ConnectUserId))
					{
						playerBalls.Add(player.ConnectUserId, new float[] { 0, 0, 0 });
					}
					else
					{
						playerBalls[player.ConnectUserId] = new float[] { 0, 0, 0 };
					}

					foreach (Player pl in Players)
					{
						if (pl.ConnectUserId != player.ConnectUserId)
						{
							player.Send("BallMove", playerId, x, y, z);
						}
					}
					break;
			}
		}
	}
}