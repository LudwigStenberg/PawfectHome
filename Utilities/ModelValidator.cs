using System.ComponentModel.DataAnnotations;
using System.Reflection;

/// <summary>
/// Validates model objects using data annotations with additional string trimming and validation.
/// </summary>
public class ModelValidator
{
    /// <summary>
    /// Validates the provided model object using data annotations and custom validation rules.
    /// Trims string properties and adds validation for empty strings and length constraints.
    /// </summary>
    /// <param name="model">The model object to validate.</param>
    /// <exception cref="ValidationFailedException">Thrown when the model fails validation.</exception>
    public void ValidateModel(object model)
    {
        if (model == null)
        {
            throw CreateValidationFailure("The model cannot be null.", "Model");
        }

        var validationContext = new ValidationContext(model);

        var validationResults = new List<ValidationResult>();

        bool isValid = Validator.TryValidateObject(
            model,
            validationContext,
            validationResults,
            validateAllProperties: true
        );

        var properties = model.GetType().GetProperties();
        foreach (var property in properties)
        {
            var value = property.GetValue(model);

            if (value is string strValue)
            {
                var trimmedValue = strValue.Trim();

                if (strValue != trimmedValue)
                {
                    property.SetValue(model, trimmedValue);
                }

                if (string.IsNullOrWhiteSpace(trimmedValue))
                {
                    validationResults.Add(
                        new ValidationResult(
                            $"{property.Name} cannot be empty or just whitespace",
                            new[] { property.Name }
                        )
                    );
                    isValid = false;
                }
                else
                {
                    var lengthAttribute = property.GetCustomAttribute<StringLengthAttribute>();
                    if (lengthAttribute != null)
                    {
                        if (
                            trimmedValue.Length < lengthAttribute.MinimumLength
                            || trimmedValue.Length > lengthAttribute.MaximumLength
                        )
                        {
                            validationResults.Add(
                                new ValidationResult(
                                    $"{property.Name} must be between {lengthAttribute.MinimumLength} and {lengthAttribute.MaximumLength} characters.",
                                    new[] { property.Name }
                                )
                            );
                            isValid = false;
                        }
                    }
                }
            }
        }
        if (!isValid)
        {
            throw ValidationFailedException.FromValidationResults(validationResults);
        }
    }

    /// <summary>
    /// Creates a ValidationFailedException with a single validation error, simplifying the creation
    /// of validation exceptions for common scenarios without requiring verbose ValidationResult creation.
    /// </summary>
    /// <param name="message">The error message describing the validation failure.</param>
    /// <param name="propertyName">The name of the property that failed validation. If empty, defaults to "Request"
    /// to indicate a request-level validation issue rather than a specific property.</param>
    /// <returns>A ValidationFailedException containing a single validation error with the specified message and property.</returns>
    public ValidationFailedException CreateValidationFailure(
        string message,
        string propertyName = ""
    )
    {
        var fieldName = string.IsNullOrEmpty(propertyName) ? "Request" : propertyName;
        var error = new ValidationResult(message, new[] { fieldName });
        return ValidationFailedException.FromValidationResults(new[] { error });
    }
}
