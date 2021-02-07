using GameLobbyApi.Contexts;
using GameLobbyApi.Entities;
using GameLobbyApi.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Linq;
using System.Text;
using System;

namespace GameLobbyApi.Services
{
	public class PlayerService
	{
		private readonly GameLobbyContext context;
		private readonly AppSettings appSettings;
		private readonly SecurityService securityService;

		public PlayerService(GameLobbyContext context, IOptions<AppSettings> appSettings, SecurityService securityService)
		{
			this.context = context;
			this.appSettings = appSettings.Value;
			this.securityService = securityService;
		}

		public Player Authenticate(string username, string password)
		{
			Player player = context.Players.SingleOrDefault(player => player.Username == username);

			if (player == null || !securityService.VerifyPassword(password, player.Password))
			{
				return null;
			}

			JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
			byte[] key = Encoding.ASCII.GetBytes(appSettings.Secret);

			SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[]
				{
					new Claim(ClaimTypes.Name, player.Id.ToString())
				}),
				Expires = DateTime.UtcNow.AddDays(1),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};

			SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
			player.Token = tokenHandler.WriteToken(token);

			return player;
		}

		public Player GetAuthorized(string id)
		{
			return GetAuthorized(Int32.Parse(id));
		}

		public Player GetAuthorized(int id)
		{
			return context.Players.Include(p => p.Room).SingleOrDefault(player => player.Id == id);
		}
	}
}