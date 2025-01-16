using System.ComponentModel.DataAnnotations;


namespace DopamineDetoxAPI.Attributes.Validation
{
    public class DateRangeAttribute : ValidationAttribute
    {
        public string StartDatePropertyName { get; set; }
        public string EndDatePropertyName { get; set; }

        public DateRangeAttribute(string startDatePropertyName, string endDatePropertyName)
        {
            StartDatePropertyName = startDatePropertyName;
            EndDatePropertyName = endDatePropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var startDateProperty = validationContext.ObjectType.GetProperty(StartDatePropertyName);
            var endDateProperty = validationContext.ObjectType.GetProperty(EndDatePropertyName);

            if (startDateProperty == null || endDateProperty == null)
            {
                return new ValidationResult($"Properties {StartDatePropertyName} or {EndDatePropertyName} not found.");
            }

            var startDateValue = startDateProperty.GetValue(validationContext.ObjectInstance) as DateTime?;
            var endDateValue = endDateProperty.GetValue(validationContext.ObjectInstance) as DateTime?;

            if (startDateValue.HasValue && endDateValue.HasValue && startDateValue > endDateValue)
            {
                return new ValidationResult($"{StartDatePropertyName} must not be later than {EndDatePropertyName}.");
            }

            return ValidationResult.Success;
        }
    }
}
