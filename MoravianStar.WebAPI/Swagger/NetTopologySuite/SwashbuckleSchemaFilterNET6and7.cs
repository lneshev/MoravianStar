#if NET6_0 || NET7_0
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;

namespace MoravianStar.WebAPI.Swagger.NetTopologySuite
{
    internal abstract class SwashbuckleSchemaFilterBase : ISchemaFilter
    {
        protected abstract GeoSerializeType Type { get; }

        protected abstract Dictionary<Type, string> Mapper { get; }

        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (!Mapper.TryGetValue(context.Type, out var value))
            {
                return;
            }

            schema.Properties.Clear();
            schema.Type = Type == GeoSerializeType.Geojson ? "object" : "string";
            schema.Example = new OpenApiString(value);
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