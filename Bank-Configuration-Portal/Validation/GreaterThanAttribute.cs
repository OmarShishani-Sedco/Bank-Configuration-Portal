using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Bank_Configuration_Portal.Validation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class GreaterThanAttribute : ValidationAttribute, IClientValidatable
    {
        public string OtherProperty { get; }
        public GreaterThanAttribute(string otherProperty) => OtherProperty = otherProperty;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var otherProp = validationContext.ObjectType.GetProperty(OtherProperty, BindingFlags.Instance | BindingFlags.Public);
            if (otherProp == null) return ValidationResult.Success;

            var otherVal = otherProp.GetValue(validationContext.ObjectInstance, null);

            if (TryToDecimal(value, out var current) && TryToDecimal(otherVal, out var other))
            {
                if (current > other) return ValidationResult.Success;
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ValidationType = "greaterthan",
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName())
            };
            rule.ValidationParameters["other"] = OtherProperty;
            yield return rule;
        }

        private static bool TryToDecimal(object input, out decimal result)
        {
            if (input == null) { result = 0; return false; }
            try { result = Convert.ToDecimal(input); return true; }
            catch { result = 0; return false; }
        }
    }
}