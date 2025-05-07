
using System.ComponentModel.DataAnnotations;

public class AdoptionApplicationEntity
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public AdoptionStatus AdoptionStatus { get; set; } = AdoptionStatus.Pending;
    public int UserId { get; set; }
    public int PetId { get; set; }

    // Navigation Props
    public UserEntity UserEntity { get; set; }
    public PetEntity PetEntity { get; set; }
}

public enum AdoptionStatus
{
    Pending,
    Declined,
    Approved,
}

// Given that `AdoptionApplicationEntity` represents an application made by a user to adopt a pet, it would be logical to assume that both `UserEntity` and 
// `PetEntity` are essential to the application's integrity. Therefore, the best approach would be to 
// make them required properties using the `required` keyword if you're using C# 11 or higher:

// ```csharp
// public class AdoptionApplicationEntity
// {
//     public int Id { get; set; }
//     public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
//     public AdoptionStatus AdoptionStatus { get; set; } = AdoptionStatus.Pending;
//     public int UserId { get; set; }
//     public int PetId { get; set; }

//     public required UserEntity UserEntity { get; set; }
//     public required PetEntity PetEntity { get; set; }
// }
// ```

// ### Why This Works Well:

// 1. **Required for Instantiation:** This guarantees that both entities are set when creating a new `AdoptionApplicationEntity`, avoiding null references.
// 2. **Clearer Intent:** It is explicitly clear that an application cannot exist without both a `UserEntity` and a `PetEntity`.
// 3. **Avoids Null Warnings:** You won't see nullability warnings, as the compiler ensures they are always initialized.

// ---

// Would you like me to also show how this would be instantiated correctly, including best practices for Entity Framework?
