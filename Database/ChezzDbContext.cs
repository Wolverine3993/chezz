using Chezz.Database.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Chezz.Database
{
	public class ChezzDbContext : IdentityDbContext<ChezzUser, ChezzRole, string>
	{
		public ChezzDbContext(DbContextOptions<ChezzDbContext> options) : base(options) { }
		public DbSet<UserRelationship> UserRelationships { get; set; }
		public DbSet<Notification> Notifications { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<UserRelationship>()
				.HasIndex(x => new { x.User1Id, x.User2Id })
				.IsUnique();

			modelBuilder.Entity<UserRelationship>()
				.HasIndex(x => x.User1Id);

			modelBuilder.Entity<Notification>()
				.HasIndex(x => x.UserId);

			base.OnModelCreating(modelBuilder);
		}
	}
}
