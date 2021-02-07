using GameLobbyApi.Entities;
using System;

namespace GameLobbyApi.Mappers
{
	public static class Dto
	{
		public static GameLobbyApi.Dto.Player MapPlayer(Player player)
		{
			ShortGuid shortGuid = player.Guid;

			return new GameLobbyApi.Dto.Player()
			{
				Id = shortGuid.Value,
				Name = player.Name,
			};
		}

		public static GameLobbyApi.Dto.Room MapRoom(Room room)
		{
			ShortGuid shortGuid = room.Guid;

			return new GameLobbyApi.Dto.Room()
			{
				Id = shortGuid.Value,
				Name = room.Name,
				OwnerName = room.Owner.Name,
				NumberOfPlayers = room.Players.Count,
			};
		}
	}
}
