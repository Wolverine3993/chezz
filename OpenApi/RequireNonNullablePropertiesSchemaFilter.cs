using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Chezz.OpenApi;

public class RequireNonNullablePropertiesSchemaFilter : ISchemaFilter
{
	public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
	{
		if (schema is not OpenApiSchema concrete || concrete.Properties is null)
			return;

		foreach ((string name, IOpenApiSchema property) in concrete.Properties)
		{
			bool isNullable = property is OpenApiSchema inline
				&& inline.Type is { } type
				&& type.HasFlag(JsonSchemaType.Null);

			if (isNullable)
				continue;

			concrete.Required ??= new HashSet<string>();
			concrete.Required.Add(name);
		}
	}
}
