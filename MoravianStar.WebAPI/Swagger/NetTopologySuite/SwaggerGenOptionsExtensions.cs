using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;

namespace MoravianStar.WebAPI.Swagger.NetTopologySuite
{
    public static class SwaggerGenOptionsExtensions
    {
        public static void AddGeometry(this SwaggerGenOptions options, GeoSerializeType type)
        {
            switch (type)
            {
                case GeoSerializeType.Geojson:
                    options.SchemaFilter<SwashbuckleGeojsonSchemaFilter>();
                    break;

                case GeoSerializeType.Wkt:
                    options.SchemaFilter<SwashbuckleWktSchemaFilter>();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}