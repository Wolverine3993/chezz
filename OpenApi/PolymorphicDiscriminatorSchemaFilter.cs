using System.Reflection;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Chezz.OpenApi;

public class PolymorphicDiscriminatorSchemaFilter : ISchemaFilter
{
	public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
	{
		Type? baseType = context.Type.BaseType;
		if (baseType is null)
			return;

		JsonPolymorphicAttribute? polymorphic = baseType.GetCustomAttribute<JsonPolymorphicAttribute>();
		if (polymorphic is null)
			return;

		JsonDerivedTypeAttribute? derived = baseType
			.GetCustomAttributes<JsonDerivedTypeAttribute>()
			.FirstOrDefault(a => a.DerivedType == context.Type);
		if (derived?.TypeDiscriminator is not string discriminator)
			return;

		string propertyName = polymorphic.TypeDiscriminatorPropertyName ?? "$type";

		if (schema is not OpenApiSchema concrete)
			return;

		OpenApiSchema target = concrete.AllOf?
			.OfType<OpenApiSchema>()
			.FirstOrDefault(s => s.Properties is not null) ?? concrete;

		target.Properties ??= new Dictionary<string, IOpenApiSchema>();
		target.Properties[propertyName] = new OpenApiSchema
		{
			Type = JsonSchemaType.String,
			Enum = new List<JsonNode> { discriminator }
		};
	}
}
