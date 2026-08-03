using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Chezz.OpenApi;

public class RequireNonNullablePropertiesSchemaFilter : ISchemaFilter
{
	public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
	{
		Require(schema);
	}

	private static void Require(IOpenApiSchema schema)
	{
		if (schema is not OpenApiSchema concrete)
			return;

		if (concrete.AllOf is not null)
		{
			foreach (IOpenApiSchema member in concrete.AllOf)
				Require(member);
		}

		if (concrete.Properties is null)
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
