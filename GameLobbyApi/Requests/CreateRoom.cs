using System.ComponentModel.DataAnnotations;

namespace GameLobbyApi.Requests
{
	public class CreateRoom
	{
		[Required]
		[StringLength(16, MinimumLength = 3)]
		[RegularExpression("([a-zA-Z0-9]+)")]
		public string Name { get; set; }
	}
}