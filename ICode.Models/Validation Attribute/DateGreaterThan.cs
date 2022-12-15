using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ICode.Models.Validation_Attribute
{
    public class DateGreaterThanAttribute: ValidationAttribute
    {
        public string CompareToField { get; set; }
        public DateGreaterThanAttribute(string compareToField, string errorMessage)
        {
            CompareToField = compareToField;
            ErrorMessage = errorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime thisDay = (DateTime)value;

            DateTime thatDay = (DateTime)validationContext.ObjectType.GetProperty(CompareToField).GetValue(validationContext.ObjectInstance, null);

            if (thisDay > thatDay)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(ErrorMessage);
            }
        }
    }
}
