using GameLobbyApi.Entities;
using System;

namespace GameLobbyApi.Responses
{
	public class AuthenticatePlayer
	{
		public string Id { get; set; }

		public string Name { get; set; }

		public string Token { get; set; }

		public AuthenticatePlayer(Player player)
		{
			ShortGuid shortGuid = player.Guid;

			Id = shortGuid.Value;
			Name = player.Name;
			Token = player.Token;
		}
	}
}