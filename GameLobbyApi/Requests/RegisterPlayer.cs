using System.ComponentModel.DataAnnotations;

namespace GameLobbyApi.Requests
{
	public class RegisterPlayer
	{
		[Required]
		[StringLength(32, MinimumLength = 4)]
		[RegularExpression("([a-zA-Z0-9]+)")]
		public string Username { get; set; }

		[Required]
		[StringLength(32, MinimumLength = 8)]
		public string Password { get; set; }
	}
}