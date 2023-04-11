using Microsoft.AspNetCore.Routing;
using System;
using System.Text.RegularExpressions;

namespace MoravianStar.WebAPI.Transformers
{
    /// <summary>
    /// <see href="https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/routing?view=aspnetcore-3.0#use-a-parameter-transformer-to-customize-token-replacement-1"/>
    /// </summary>
    public class SlugifyParameterTransformer : IOutboundParameterTransformer
    {
        public string TransformOutbound(object value)
        {
            if (value == null)
            {
                return null;
            }
            return regex.Replace(value.ToString(), "$1-$2").ToLowerInvariant();
        }

        private static readonly Regex regex = new("([a-z])([A-Z])", RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(100));
    }
}