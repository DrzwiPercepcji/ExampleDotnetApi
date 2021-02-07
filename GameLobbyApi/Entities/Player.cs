using System.ComponentModel.DataAnnotations;

namespace GameLobbyApi.Entities
{
	public class Player : AbstractEntity
	{
		[Required]
		[StringLength(16)]
		public string Name { get; set; }

		[Required]
		[StringLength(32)]
		public string Username { get; set; }

		[Required]
		public string Password { get; set; }

		public string Token { get; set; }

		public Room Room { get; set; }
	}
}