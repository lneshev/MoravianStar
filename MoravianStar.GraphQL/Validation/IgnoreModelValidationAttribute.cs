using System;

namespace MoravianStar.GraphQL.Validation
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class IgnoreModelValidationAttribute : Attribute { }
}
