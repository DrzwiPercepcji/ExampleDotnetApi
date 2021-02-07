using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GameLobbyApi.Responses;
using GameLobbyApi.Contexts;
using GameLobbyApi.Entities;
using GameLobbyApi.Services;
using System.Collections.Generic;
using System.Linq;
using System;

namespace GameLobbyApi.Controllers.V1
{
	[Authorize]
	[ApiController]
	[ApiVersion("1.0")]
	[Route("[controller]")]
	public class PlayersController : ControllerBase
	{
		private readonly GameLobbyContext context;
		private readonly PlayerService playerService;
		private readonly SecurityService securityService;

		public PlayersController(GameLobbyContext context, PlayerService playerService, SecurityService securityService)
		{
			this.context = context;
			this.playerService = playerService;
			this.securityService = securityService;
		}

		[AllowAnonymous]
		[HttpPost("register/guest")]
		public ActionResult<Register> RegisterQuest()
		{
			ShortGuid shortGuid = Guid.NewGuid();
			string password = securityService.GeneratePassword();
			string encrypted = securityService.HashPassword(password);

			Player player = new Player()
			{
				Guid = shortGuid.Guid,
				Name = "Player",
				Username = "player_" + shortGuid.Value,
				Password = encrypted,
			};

			context.Players.Add(player);
			context.SaveChanges();

			return new Register()
			{
				Username = player.Username,
				Password = password,
			};
		}

		[AllowAnonymous]
		[HttpPost("register")]
		public ActionResult Register(Requests.RegisterPlayer request)
		{
			string encrypted = securityService.HashPassword(request.Password);

			List<Player> players = context.Players
				.Where(p => p.Username == request.Username)
				.ToList();

			if (players.Count > 0)
			{
				return Conflict();
			}

			Player player = new Player()
			{
				Name = "Player",
				Username = request.Username,
				Password = encrypted,
			};

			context.Players.Add(player);
			context.SaveChanges();

			return Ok();
		}

		[AllowAnonymous]
		[HttpPost("authenticate")]
		public ActionResult<Responses.AuthenticatePlayer> Authenticate(Requests.AuthenticatePlayer authenticate)
		{
			Player player = playerService.Authenticate(authenticate.Username, authenticate.Password);

			if (player == null)
			{
				return NotFound();
			}

			return new Responses.AuthenticatePlayer(player);
		}

		[HttpPatch("edit")]
		public ActionResult Edit(Requests.EditProfile request)
		{
			Player player = playerService.GetAuthorized(User.Identity.Name);

			player.Name = request.Name;

			context.Entry(player).State = EntityState.Modified;
			context.SaveChanges();

			return Ok();
		}
	}
}
