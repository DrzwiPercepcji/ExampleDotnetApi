namespace GameLobbyApi.Dto
{
	public class Room
	{
		public string Id { get; set; }

		public string Name { get; set; }

		public string OwnerName { get; set; }

		public int NumberOfPlayers { get; set; }
	}
}
