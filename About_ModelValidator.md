## **1️⃣ ValidationError Class**

```csharp
public class ValidationError
{
    public string PropertyName { get; set; }
    public string ErrorMessage { get; set; }

    public ValidationError(string propertyName, string errorMessage)
    {
        PropertyName = propertyName ?? "General";
        ErrorMessage = errorMessage;
    }
}
```

### **What is it?**

`ValidationError` is a **plain old C# object (POCO)** designed to represent a single validation error. It consists of:

- `PropertyName`: The name of the property that failed validation.
    
- `ErrorMessage`: A descriptive error message explaining the issue.
    

### **Why use it?**

This class is a clean way to encapsulate information about a validation error:

- It makes it easy to list errors later.
    
- It's a structured way to communicate what went wrong and where.
    

### **The Constructor**

The constructor takes two parameters:

- `propertyName`: If it's `null`, it defaults to `"General"`. This happens through the `??` operator, which is the **null-coalescing operator**.
    
- `errorMessage`: This is simply assigned as-is.
    

If `propertyName` is null, the error is considered **general**—not tied to a specific property.

---

## **2️⃣ ValidationFailedException Class**

```csharp
public class ValidationFailedException : Exception
{
    public IReadOnlyList<ValidationError> Errors { get; }
```

### **What is it?**

`ValidationFailedException` is a **custom exception** that inherits from the base `Exception` class. It's specifically designed to represent a collection of validation errors.

- `Errors`: This is a **read-only list** (`IReadOnlyList`) of `ValidationError` objects that store information about each failed validation.
    

---

### **The Constructor**

```csharp
public ValidationFailedException(string message, IEnumerable<ValidationError> errors)
    : base(message)
{
    Errors = errors.ToList().AsReadOnly();
}
```

1. `string message`: This is the main error message for the exception.
    
2. `IEnumerable<ValidationError> errors`: A collection of `ValidationError` objects.
    
3. `base(message)`: This calls the constructor of `Exception`, passing the message upwards so it is accessible like a normal exception.
    
4. `errors.ToList().AsReadOnly()`:
    
    - `.ToList()`: Converts the `IEnumerable` to a `List`.
        
    - `.AsReadOnly()`: Wraps it as a **read-only collection**. This means the list cannot be modified after creation—important for data integrity.
        

---

### **Static Factory Method**

```csharp
public static ValidationFailedException FromValidationResults(
    IEnumerable<ValidationResult> validationResults,
    string message = "Validation failed"
)
```

This is a **factory method**—a static method that helps construct the object easily.

1. `IEnumerable<ValidationResult> validationResults`:
    
    - This is the list of errors collected during validation (from `Validator.TryValidateObject()`).
        
2. The method **transforms** each `ValidationResult` into a `ValidationError`:
    
    ```csharp
    var errors = validationResults
         .Select(r => new ValidationError(
             r.MemberNames.FirstOrDefault() ?? "General",
             r.ErrorMessage ?? "Unknown error"
         ))
         .ToList();
    ```
    
    - `Select`: Projects each `ValidationResult` into a `ValidationError`.
        
    - `r.MemberNames.FirstOrDefault()`: Takes the first property name that failed (if there is one).
        
    - If there is no property name, it defaults to `"General"`.
        
    - If there is no error message, it defaults to `"Unknown error"`.
        
3. Finally, it **returns** a new `ValidationFailedException` populated with the list of errors.
    

---

### **Overriding `ToString()`**

```csharp
public override string ToString()
{
    var errorMessages = string.Join(
        Environment.NewLine,
        Errors.Select(e => $"- {e.PropertyName}: {e.ErrorMessage}")
    );

    return $"{Message}{Environment.NewLine}{errorMessages}";
}
```

- This overrides the default string representation of the exception.
    
- It formats each `ValidationError` in a clean list format:
    
    ```
    - PropertyName: ErrorMessage
    ```
    
- It joins them with `Environment.NewLine`, which is a cross-platform way to do line breaks (`\n` or `\r\n` depending on OS).
    

---

## **3️⃣ ModelValidator Class**

```csharp
public class ModelValidator
{
    public void ValidateModel(object model)
    {
```

### **What is it?**

`ModelValidator` is a service that:

- Takes a model (any object) as input.
    
- Validates its properties using:
    
    1. **Data Annotations** (like `[Required]`, `[StringLength]`, etc.).
        
    2. **Custom logic** for null, empty, or whitespace-only checks.
        

---

### **1️⃣ Data Annotations Validation**

```csharp
var validationContext = new ValidationContext(model);
var validationResults = new List<ValidationResult>();

bool isValid = Validator.TryValidateObject(
    model,
    validationContext,
    validationResults,
    validateAllProperties: true
);
```

1. `ValidationContext`: Encapsulates information about the object to validate.
    
2. `validationResults`: This collects any errors that occur during validation.
    
3. `Validator.TryValidateObject()`:
    
    - **model**: The object to validate.
        
    - **validationContext**: The context of the object.
        
    - **validationResults**: A list that will be populated with errors.
        
    - `validateAllProperties: true`: Ensures every property is checked.
        

If the object violates any Data Annotations, `isValid` becomes `false`.

---

### **2️⃣ Custom Logic for Null/Whitespace Checks**

```csharp
var properties = model.GetType().GetProperties();
foreach (var property in properties)
{
    var value = property.GetValue(model);

    if (value is string strValue && string.IsNullOrWhiteSpace(strValue))
    {
        validationResults.Add(new ValidationResult($"{property.Name} cannot be empty or just whitespace.", new[] { property.Name }));
        isValid = false;
    }
    else if (value == null && property.PropertyType == typeof(string))
    {
        validationResults.Add(new ValidationResult($"{property.Name} cannot be null.", new[] { property.Name }));
        isValid = false;
    }
}
```

1. `model.GetType().GetProperties()`: Reflectively gets all properties of the model.
    
2. Loops through each property:
    
    - If it is a **string** and **whitespace-only** → adds a validation error.
        
    - If it is `null` and expected to be a string → adds a validation error.
        

This allows you to catch cases even if there are **no Data Annotations** present.

---

### **3️⃣ Throw ValidationFailedException if Invalid**

```csharp
if (!isValid)
{
    throw ValidationFailedException.FromValidationResults(validationResults);
}
```

If any error is found, it throws a `ValidationFailedException`, using the factory method to wrap the errors in a structured format.

---

### **Summary**

1. **ValidationError:** Represents a single error.
    
2. **ValidationFailedException:** Collects all errors and provides readable output.
    
3. **ModelValidator:** Central service that:
    
    - Uses **Data Annotations** for built-in validation.
        
    - Adds **custom checks** for null/whitespace.
        
    - Throws a well-structured exception if validation fails.
        

---



The `ValidationContext` is essentially a wrapper around your model that provides the validation engine with **metadata** about the object being validated.

---

### **How Does It Work?**

When you create a `ValidationContext`, you pass in the model instance that you want to validate. The `ValidationContext` then:

1. Reflects on the model to discover:
    
    - All its properties.
        
    - Any **Data Annotations** (like `[Required]`, `[StringLength]`, `[Range]`, etc.).
        
2. Makes this metadata available to the `Validator.TryValidateObject()` method.
    

---

### **Example**

Here's a simple example to illustrate:

```csharp
public class RegisterPetRequest
{
    [Required(ErrorMessage = "Name cannot be null")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "The pet name must be between 3 and 50 characters.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Age is required.")]
    [Range(0, 20, ErrorMessage = "Age must be between 0 and 20.")]
    public int Age { get; set; }
}
```

Now, let's validate this using `ValidationContext` and `Validator.TryValidateObject()`:

```csharp
var pet = new RegisterPetRequest
{
    Name = "Bo",
    Age = 25 // Invalid, out of range
};

var validationContext = new ValidationContext(pet); // 🔹 Creates the context around the model
var validationResults = new List<ValidationResult>();

bool isValid = Validator.TryValidateObject(
    pet,
    validationContext,
    validationResults,
    validateAllProperties: true
);

if (!isValid)
{
    foreach (var result in validationResults)
    {
        Console.WriteLine($"{result.MemberNames.First()}: {result.ErrorMessage}");
    }
}
```

---

### **Output**

```
Name: The pet name must be between 3 and 50 characters.
Age: Age must be between 0 and 20.
```

---

### **What Happens Here?**

1. `ValidationContext` takes the `pet` instance and inspects its properties.
    
2. `Validator.TryValidateObject()`:
    
    - Reads the **Data Annotations** (`[Required]`, `[StringLength]`, `[Range]`).
        
    - Checks each property against its rules.
        
    - Populates `validationResults` with any issues.
        
3. It finally returns `false` if any validation errors are found.
    

---

### **Why Is It Useful?**

1. **Metadata-Driven:** It uses the attributes directly from your class, keeping your model definitions and validation rules together.
    
2. **Reusability:** You can pass the same `ValidationContext` to multiple validators if you want custom logic.
    
3. **Separation of Concerns:** It separates _how the object is validated_ from _the object itself_.
    

---

