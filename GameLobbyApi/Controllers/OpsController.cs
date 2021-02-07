using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace GameLobbyApi.Controllers
{
	[ApiController]
	[ApiVersion("1.0")]
	[Route("[controller]")]
	public class OpsController : ControllerBase
	{
		[HttpGet("health")]
		public Dictionary<string, string> Get()
		{
			return new Dictionary<string, string>()
			{
				["status"] = "OK"
			};
		}
	}
}