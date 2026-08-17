using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Chezz.Game.Games.Chess;

public class ChessGameStateSchemaFilter : ISchemaFilter
{
	public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
	{
		if (context.Type != typeof(ChessGameState))
			return;

		if (schema.Properties is not null &&
			schema.Properties.TryGetValue("board", out var board) &&
			board.Items is OpenApiSchema row &&
			row.Items is { } cellRef)
		{

			row.Items = new OpenApiSchema
			{
				Type = JsonSchemaType.Null,
				AllOf = new List<IOpenApiSchema> { cellRef }
			};
		}
	}
}
