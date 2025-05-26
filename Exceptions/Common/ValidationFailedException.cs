using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents a single validation error for a property.
/// </summary>
public class ValidationError
{
    public string PropertyName { get; set; }

    public string ErrorMessage { get; set; }

    /// <summary>
    /// Initializes a new instance of the ValidationError class.
    /// </summary>
    /// <param name="propertyName">The name of the property that failed validation (defaults to "General" if null).</param>
    /// <param name="errorMessage">The error message describing the validation failure.</param>
    public ValidationError(string propertyName, string errorMessage)
    {
        PropertyName = propertyName ?? "General";

        ErrorMessage = errorMessage;
    }
}

/// <summary>
/// Exception thrown when model validation fails.
/// </summary>
public class ValidationFailedException : Exception
{
    public IReadOnlyList<ValidationError> Errors { get; }

    /// <summary>
    /// Initializes a new instance of the ValidationFailedException class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="errors">The collection of validation errors.</param>
    public ValidationFailedException(string message, IEnumerable<ValidationError> errors)
        : base(message)
    {
        Errors = errors.ToList().AsReadOnly();
    }

    /// <summary>
    /// Creates a ValidationFailedException from a collection of ValidationResult objects.
    /// </summary>
    /// <param name="validationResults">The validation results to convert.</param>
    /// <param name="message">The exception message (defaults to "Validation failed").</param>
    /// <returns>A new ValidationFailedException containing the validation errors.</returns>
    public static ValidationFailedException FromValidationResults(
        IEnumerable<ValidationResult> validationResults,
        string message = "Validation failed"
    )
    {
        var errors = validationResults
            .Select(r => new ValidationError(
                r.MemberNames.FirstOrDefault() ?? "General",
                r.ErrorMessage ?? "Unknown error"
            ))
            .ToList();

        return new ValidationFailedException(message, errors);
    }

    /// <summary>
    /// Returns a string representation of the validation exception with all error details.
    /// </summary>
    public override string ToString()
    {
        var errorMessages = string.Join(
            Environment.NewLine,
            Errors.Select(e => $"- {e.PropertyName}: {e.ErrorMessage}")
        );

        return $"{Message}{Environment.NewLine}{errorMessages}";
    }
}