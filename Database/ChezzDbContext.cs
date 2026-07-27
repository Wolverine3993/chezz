using Chezz.Database.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Chezz.Database
{
	public class ChezzDbContext : IdentityDbContext<ChezzUser, ChezzRole, string>
	{
		public ChezzDbContext(DbContextOptions<ChezzDbContext> options) : base(options) { }
	}
}
