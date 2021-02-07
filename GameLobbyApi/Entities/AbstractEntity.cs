using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GameLobbyApi.Entities
{
	public abstract class AbstractEntity
	{
		[Key]
		public int Id { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Guid { get; set; }

		public DateTime Created { get; set; }

		public DateTime Updated { get; set; }
	}
}
