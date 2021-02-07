using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using GameLobbyApi.Entities;
using System;

namespace GameLobbyApi.Contexts
{
	public class GameLobbyContext : DbContext
	{
		public GameLobbyContext(DbContextOptions<GameLobbyContext> options) : base(options) { }

		public DbSet<Room> Rooms { get; set; }

		public DbSet<Player> Players { get; set; }

		public override int SaveChanges()
		{
			DateTime now = DateTime.Now;

			foreach (EntityEntry changedEntity in ChangeTracker.Entries())
			{
				if (changedEntity.Entity is AbstractEntity entity)
				{
					switch (changedEntity.State)
					{
						case EntityState.Added:
							entity.Created = now;
							entity.Updated = now;
							break;
						case EntityState.Modified:
							Entry(entity).Property(e => e.Created).IsModified = false;
							entity.Updated = now;
							break;
					}
				}
			}

			return base.SaveChanges();
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Player>().HasAlternateKey(p => p.Guid);
			modelBuilder.Entity<Player>().HasIndex(p => p.Username).IsUnique();

			modelBuilder.Entity<Room>().HasAlternateKey(r => r.Guid);

			modelBuilder.Entity<Room>()
				.HasMany(r => r.Players)
				.WithOne(p => p.Room)
				.OnDelete(DeleteBehavior.SetNull);

			modelBuilder.Entity<Room>()
				.HasOne(r => r.Owner);
		}
	}
}