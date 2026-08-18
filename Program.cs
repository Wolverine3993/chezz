using Chezz.Database;
using Chezz.Database.EntityManagers;
using Chezz.Database.Models;
using Chezz.Game;
using Chezz.Game.Games.Chess;
using Chezz.Notifications;
using Chezz.RequestSchemas.Identity;
using Chezz.SMTP;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json.Serialization;

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
				options.Conventions.Add(new Conventions.RoutePrefixConvention("api"));
			});

			var registerCode = builder.Configuration.GetRequiredSection("RegisterCode");
            if (registerCode.Value is not null)
			{
				builder.Services.AddSingleton(new RegisterCode
				{
					Value = registerCode.Value
				});
			}

			builder.Services.AddDbContext<ChezzDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
			builder.Services.AddScoped<UserRelationshipManager>();
			builder.Services.AddScoped<NotificationManager>();
			builder.Services.AddSingleton<NotificationSocketRegistry>();

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
				options.CustomOperationIds(apiDescription =>
				{
					if (apiDescription.ActionDescriptor is not ControllerActionDescriptor descriptor)
						return null;

					var action = descriptor.AttributeRouteInfo?.Name ?? descriptor.ActionName;
					return $"{descriptor.ControllerName}_{action}";
				});

				options.UseAllOfForInheritance();
				options.UseOneOfForPolymorphism();
				options.SelectDiscriminatorNameUsing(baseType =>
					baseType.GetCustomAttribute<JsonPolymorphicAttribute>()?.TypeDiscriminatorPropertyName);
				options.SelectDiscriminatorValueUsing(subType =>
					subType.BaseType?
						.GetCustomAttributes<JsonDerivedTypeAttribute>()
						.FirstOrDefault(a => a.DerivedType == subType)?
						.TypeDiscriminator?.ToString());

				options.SupportNonNullableReferenceTypes();
				options.SchemaFilter<Chezz.OpenApi.PolymorphicDiscriminatorSchemaFilter>();
				options.SchemaFilter<Chezz.OpenApi.RequireNonNullablePropertiesSchemaFilter>();

				options.SchemaFilter<ChessGameStateSchemaFilter>();
			});

			// Game registries
			builder.Services.AddSingleton<LobbyRegistry>();
			builder.Services.AddSingleton<GameRegistry<ChessMove, ChessPiece, ChessGameState, ChessGameStore, ChessGameImplementation>>();

			// Global error handling
			builder.Services.AddProblemDetails();
			builder.Services.AddExceptionHandler<Errors.AppExceptionHandler>();

			if (builder.Environment.IsDevelopment())
			{
				builder.Services.AddCors(options =>
				{
					options.AddDefaultPolicy(policy =>
					{
						policy.WithOrigins("http://localhost:3000")
						.AllowAnyMethod()
						.AllowCredentials()
						.AllowAnyHeader();
					});
				});
			}

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
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
