using GameLobbyApi.Entities;
using System.Collections.Generic;
using System.Linq;
using System;

namespace GameLobbyApi.Responses
{
	public class GetRoom
	{
		public string Id { get; set; }

		public string Name { get; set; }

		public Dto.Player Owner { get; set; }

		public List<Dto.Player> Players { get; set; }

		public GetRoom(Room room) : this(room, room.Players.Select(p => Mappers.Dto.MapPlayer(p)).ToArray()) { }

		public GetRoom(Room room, Dto.Player[] players)
		{
			ShortGuid shortGuid = room.Guid;

			Id = shortGuid.Value;
			Name = room.Name;
			Owner = Mappers.Dto.MapPlayer(room.Owner);
			Players = players.ToList();
		}
	}
}