using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Chezz.Game.Games.Chess;

// Marks empty board squares as nullable so they pass schema validation
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
			// A $ref cannot be nullable directly, so wrap it in a nullable allOf
			row.Items = new OpenApiSchema
			{
				Type = JsonSchemaType.Null,
				AllOf = new List<IOpenApiSchema> { cellRef }
			};
		}
	}
}
