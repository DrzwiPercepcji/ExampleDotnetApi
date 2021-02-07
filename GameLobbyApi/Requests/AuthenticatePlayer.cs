using System.ComponentModel.DataAnnotations;

namespace GameLobbyApi.Requests
{
	public class AuthenticatePlayer
	{
		[Required]
		public string Username { get; set; }

		[Required]
		public string Password { get; set; }
	}
}