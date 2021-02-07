using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace GameLobbyApi.Entities
{
	public class Room : AbstractEntity
	{
		[Required]
		[StringLength(16)]
		public string Name { get; set; }

		public Player Owner { get; set; }

		public List<Player> Players { get; set; }
	}
}