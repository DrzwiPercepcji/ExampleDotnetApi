using GameLobbyApi.Entities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GameLobbyApi.Responses
{
	public class GetRooms : IEnumerable<Dto.Room>
	{
		public List<Dto.Room> Rooms { get; set; }

		public GetRooms(List<Room> rooms)
		{
			Rooms = rooms.Select(r => Mappers.Dto.MapRoom(r)).ToList();
		}

		public Dto.Room this[int index]
		{
			get { return Rooms[index]; }
			set { Rooms.Insert(index, value); }
		}

		public IEnumerator<Dto.Room> GetEnumerator()
		{
			return Rooms.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}