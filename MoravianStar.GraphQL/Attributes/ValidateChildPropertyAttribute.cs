using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MoravianStar.GraphQL.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ValidateChildPropertyAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();

            if (value is IEnumerable<object>)
            {
                var list = ((IEnumerable<object>)value).ToList();
                for (var i = 0; i < list.Count; i++)
                {
                    var v = list[i];
                    if (v != null && !Validator.TryValidateObject(v, new ValidationContext(v), errors, true))
                    {
                        return new ValidationResult(errors[0].ErrorMessage, new[] { $"{validationContext.MemberName}:[{i}]:{errors[0].MemberNames.ElementAt(0)}" });
                    }
                }
            }
            else
            {
                if (value != null && !Validator.TryValidateObject(value, new ValidationContext(value), errors, true))
                {
                    return new ValidationResult(errors[0].ErrorMessage, new[] { $"{validationContext.MemberName}:{errors[0].MemberNames.ElementAt(0)}" });
                }
            }

            return ValidationResult.Success;
        }
    }
}