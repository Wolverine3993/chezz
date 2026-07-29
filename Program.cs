
using Chezz.Database;
using Chezz.Database.Models;
using Chezz.Game;
using Chezz.Game.Games.Chess;
using Chezz.SMTP;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Chezz.SMTP;
using Chezz.Identity;

namespace Chezz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
			// Add services to the container.

			builder.Services.AddLogging();
			builder.Services.AddControllers(options =>
			{
				// Serve every controller under a global "/api" prefix.
				options.Conventions.Add(new Conventions.RoutePrefixConvention("api"));
			});


			builder.Services.AddDbContext<ChezzDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

			builder.Services.AddSmtpConfiguration(builder.Configuration.GetRequiredSection("SmtpConfiguration"));
			builder.Services.AddSingleton<IEmailSender, EmailSender>();

			builder.Services.AddIdentityApiEndpoints<ChezzUser>()
				.AddRoles<ChezzRole>()
				.AddEntityFrameworkStores<ChezzDbContext>()
				.AddDefaultTokenProviders();

			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen(options =>
			{
				// operationId => "{ConcreteController}_{RouteName ?? ActionName}"
				// The controller prefix keeps ids unique across concrete controllers
				// that inherit endpoints from the generic GameController base.
				options.CustomOperationIds(apiDescription =>
				{
					if (apiDescription.ActionDescriptor is not ControllerActionDescriptor descriptor)
						return null;

					var action = descriptor.AttributeRouteInfo?.Name ?? descriptor.ActionName;
					return $"{descriptor.ControllerName}_{action}";
				});
			});

			// Game registries
			builder.Services.AddSingleton<LobbyRegistry>();
			builder.Services.AddSingleton<GameRegistry<ChessMove, ChessPiece, ChessGameState, ChessGameStore, ChessGameImplementation>>();

			// Global error handling
			builder.Services.AddProblemDetails();
			builder.Services.AddExceptionHandler<Errors.AppExceptionHandler>();

			builder.Services.AddCors(options =>
			{
				options.AddDefaultPolicy(policy =>
				{
					policy.WithOrigins("http://localhost:3000")
					.AllowAnyOrigin()
					.AllowAnyMethod()
					.AllowAnyHeader();
				});
			});

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
			app.UseWebSockets();
			app.UseCors();

			app.UseExceptionHandler();

			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllers();

			app.Run();
		}
	}
}
