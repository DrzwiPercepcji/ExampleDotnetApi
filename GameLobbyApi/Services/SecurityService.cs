using GameLobbyApi.Helpers;
using Sodium;

namespace GameLobbyApi.Services
{
	public class SecurityService
	{
		public string GeneratePassword()
		{
			return Password.Generate(32, 12);
		}

		public string HashPassword(string password)
		{
			return PatchEncoding(PasswordHash.ArgonHashString(password));
		}

		public bool VerifyPassword(string password, string hash)
		{
			return PasswordHash.ArgonHashStringVerify(hash, password);
		}

		private string PatchEncoding(string hash)
		{
			return hash.Replace("\0", string.Empty);
		}
	}
}
