
using Chezz.Database;
using Chezz.Database.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Chezz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			builder.Services.AddControllers(options =>
			{
				// Serve every controller under a global "/api" prefix.
				options.Conventions.Add(new Chezz.Conventions.RoutePrefixConvention("api"));
			});

			builder.Services.AddDbContext<ChezzDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

			builder.Services.AddIdentityApiEndpoints<ChezzUser>()
				.AddRoles<ChezzRole>()
				.AddEntityFrameworkStores<ChezzDbContext>()
				.AddDefaultTokenProviders();

			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				// Apply any pending EF Core migrations automatically in development.
				using (var scope = app.Services.CreateScope())
				{
					var db = scope.ServiceProvider.GetRequiredService<ChezzDbContext>();
					db.Database.Migrate();
				}

				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.UseAuthentication();
			app.UseAuthorization();


			app.MapControllers();

			app.MapGroup("/api/identity")
				.MapIdentityApi<ChezzUser>();

			app.Run();
		}
	}
}
