namespace GameLobbyApi.Responses
{
	public class CreateRoom
	{
		public string Id { get; set; }

		public CreateRoom(string id)
		{
			Id = id;
		}
	}
}