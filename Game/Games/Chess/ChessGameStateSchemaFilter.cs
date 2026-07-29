using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Chezz.Game.Games.Chess;

/// <summary>
/// Swashbuckle does not detect the nullability of the element type inside a
/// jagged array (<c>ChessPiece?[][]</c>), so empty board squares (serialized as
/// <c>null</c>) fail client-side schema validation. This filter marks the
/// innermost board items as nullable.
/// </summary>
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
			// The board cell is a $ref to ChessPiece. In OpenAPI 3.0 a $ref
			// cannot carry a sibling "nullable" flag, so wrap it in an allOf
			// and mark the wrapper nullable to allow empty squares (null).
			row.Items = new OpenApiSchema
			{
				Type = JsonSchemaType.Null,
				AllOf = new List<IOpenApiSchema> { cellRef }
			};
		}
	}
}
