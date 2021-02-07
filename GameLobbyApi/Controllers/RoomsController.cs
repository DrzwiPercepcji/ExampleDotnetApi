using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameLobbyApi.Contexts;
using GameLobbyApi.Services;
using GameLobbyApi.Entities;
using System.Linq;
using System;

namespace GameLobbyApi.Controllers
{
	[Authorize]
	[ApiController]
	[ApiVersion("1.0")]
	[Route("[controller]")]
	public class RoomsController : ControllerBase
	{
		const int minimumPlayers = 2;
		const int maximumPlayers = 5;

		private readonly GameLobbyContext context;
		private readonly PlayerService playerService;

		public RoomsController(GameLobbyContext context, PlayerService playerService)
		{
			this.context = context;
			this.playerService = playerService;
		}

		[HttpGet]
		public ActionResult<Responses.GetRooms> GetRooms()
		{
			Player player = playerService.GetAuthorized(User.Identity.Name);

			List<Room> rooms = context.Rooms
				.Where(r => r.Players.Count > 0 && r.Owner != null)
				.Include(r => r.Players)
				.Include(r => r.Owner)
				.ToList();

			return new Responses.GetRooms(rooms);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Responses.GetRoom>> GetRoom(string id)
		{
			ShortGuid shortGuid;
			try
			{
				shortGuid = id;
			}
			catch (FormatException)
			{
				return BadRequest();
			}

			Room room = await context.Rooms
				.Include(r => r.Owner)
				.Include(r => r.Players)
				.FirstOrDefaultAsync(r => r.Guid == shortGuid.Guid);

			if (room == null)
			{
				return NotFound();
			}

			return new Responses.GetRoom(room);
		}

		[HttpPatch("join/{id}")]
		public ActionResult<Responses.GetRoom> JoinRoom(string id)
		{
			Player player = playerService.GetAuthorized(User.Identity.Name);

			ShortGuid shortGuid;
			try
			{
				shortGuid = id;
			}
			catch (FormatException)
			{
				return BadRequest();
			}

			Room room = context.Rooms
				.Include(r => r.Owner)
				.Include(r => r.Players)
				.FirstOrDefault(r => r.Guid == shortGuid.Guid);

			if (room == null)
			{
				return NotFound();
			}

			if (!room.Players.Contains(player))
			{
				if (room.Players.Count >= maximumPlayers)
				{
					return Conflict();
				}

				room.Players.Add(player);
				context.SaveChanges();
			}

			return new Responses.GetRoom(room);
		}

		[HttpPatch("leave")]
		public ActionResult LeaveRoom()
		{
			Player player = playerService.GetAuthorized(User.Identity.Name);

			if (player.Room == null)
			{
				return NotFound();
			}

			Room room = context.Rooms
				.Include(r => r.Owner)
				.Include(r => r.Players)
				.FirstOrDefault(r => r.Id == player.Room.Id);

			if (room == null)
			{
				return NotFound();
			}

			if (!room.Players.Contains(player))
			{
				return BadRequest();
			}

			room.Players.Remove(player);
			context.SaveChanges();

			return Ok();
		}

		[HttpPost]
		public ActionResult<Responses.CreateRoom> CreateRoom(Requests.CreateRoom request)
		{
			Player player = playerService.GetAuthorized(User.Identity.Name);

			if (player.Room != null)
			{
				return BadRequest();
			}

			Room room = new Room()
			{
				Name = request.Name,
				Owner = player,
				Players = new List<Player>() { player },
			};

			context.Rooms.Add(room);
			context.SaveChanges();

			ShortGuid shortGuid = room.Guid;

			return new Responses.CreateRoom(shortGuid.Value);
		}

		[HttpDelete]
		public ActionResult DeleteRoom()
		{
			Player player = playerService.GetAuthorized(User.Identity.Name);

			if (player.Room == null)
			{
				return BadRequest();
			}

			Room room = context.Rooms
				.Include(r => r.Owner)
				.FirstOrDefault(r => r.Id == player.Room.Id);

			if (room == null)
			{
				return NotFound();
			}

			if (room.Owner.Id != player.Id)
			{
				return Forbid();
			}

			context.Rooms.Remove(room);
			context.SaveChanges();

			return Ok();
		}

		[HttpDelete("kick/{id}")]
		public ActionResult KickPlayer(string id)
		{
			Player player = playerService.GetAuthorized(User.Identity.Name);

			if (player.Room == null)
			{
				return BadRequest();
			}

			Room room = context.Rooms
				.Include(r => r.Owner)
				.Include(r => r.Players)
				.FirstOrDefault(r => r.Owner.Id == player.Id && r.Id == player.Room.Id);

			if (room == null || !room.Players.Contains(player))
			{
				return BadRequest();
			}

			ShortGuid shortGuid;
			try
			{
				shortGuid = id;
			}
			catch (FormatException)
			{
				return BadRequest();
			}

			Player kickedPlayer = room.Players.SingleOrDefault(p => p.Guid == shortGuid.Guid);

			if (kickedPlayer == null || kickedPlayer.Id == player.Id)
			{
				return BadRequest();
			}

			kickedPlayer.Room = null;
			context.Entry(kickedPlayer).State = EntityState.Modified;
			context.SaveChanges();

			return Ok();
		}
	}
}
