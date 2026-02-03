#if NET8_0_OR_GREATER
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;

namespace MoravianStar.WebAPI.Swagger.NetTopologySuite
{
    internal abstract class SwashbuckleSchemaFilterBase : ISchemaFilter
    {
        protected abstract GeoSerializeType Type { get; }

        protected abstract Dictionary<Type, string> Mapper { get; }

        public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
        {
            if (!Mapper.TryGetValue(context.Type, out var value))
            {
                return;
            }

            if (schema is OpenApiSchema concrete)
            {
                concrete.Properties.Clear();
                concrete.Type = Type == GeoSerializeType.Geojson ? JsonSchemaType.Object: JsonSchemaType.String;
                concrete.Example = value;
            }
        }
    }

    internal class SwashbuckleGeojsonSchemaFilter : SwashbuckleSchemaFilterBase
    {
        protected override GeoSerializeType Type => GeoSerializeType.Geojson;
        protected override Dictionary<Type, string> Mapper => SerializeSwaggerMappers.GeometryGeojsonMapper;
    }

    internal class SwashbuckleWktSchemaFilter : SwashbuckleSchemaFilterBase
    {
        protected override GeoSerializeType Type => GeoSerializeType.Wkt;
        protected override Dictionary<Type, string> Mapper => SerializeSwaggerMappers.GeometryWktMapper;
    }
}
#endif