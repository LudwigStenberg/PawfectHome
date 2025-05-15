# Installation packages

### Database & Entity Framework

- dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
- dotnet add package Microsoft.EntityFrameworkCore
- dotnet add package Microsoft.EntityFrameworkCore.Tools
- dotnet add package Microsoft.EntityFrameworkCore.Design

### Authentication och Authorization

- dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
- dotnet add package Microsoft.Extensions.Identity.Core
- dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer

### Api Documentation

- dotnet add package Scalar.AspNetCore


---


# How To Commit - Conventional Commits

## Commit Format

Each commit message should follow this structure:

```
<type>[optional scope]: <description>

[optional body]

[optional footer(s)]
```

###  Examples
#### Multiple commits with different types and scopes:
```
feat: add user authentication with JWT

fix(api): correct file upload validation issue

chore: update dependencies

docs(readme): add setup instructions

refactor(db): optimize folder retrieval queries

test(file): add integration test for download endpoint
```

#### Single commit with body:
```
feat(db): add AuditLog entity for tracking user activity
^--- Type (feat) and Scope (db)

Introduces a new `AuditLog` entity to store user actions such as login, data edits, and other system interactions. This entity will help with logging user activity and generating activity reports.
^--- Body: Description of what this change does and why

This change includes:
- The creation of the `AuditLog` entity with fields like `userId`, `action`, `timestamp`, and `details`.
- A new migration to create the necessary table in the database.
- Updates to the service layer to handle log entries for specific actions.
^--- Body: Detailed explanation of what was changed and what the change includes

This is part of the new audit logging feature planned for the upcoming release.
^--- Footer (optional): Additional information, links to relevant issues, or mentions of planned features

```
---

## Commit Types

| Type       | Description                                                                                               |
| ---------- | --------------------------------------------------------------------------------------------------------- |
| `feat`     | A new feature (user-visible functionality) <br>See [What Counts as a Feature?](#what-counts-as-a-feature) |
| `fix`      | A bug fix                                                                                                 |
| `docs`     | Documentation-only changes                                                                                |
| `style`    | Code style changes (formatting, whitespace, etc.) that do not affect logic                                |
| `refactor` | Code restructuring without changing functionality                                                         |
| `perf`     | Performance improvements                                                                                  |
| `test`     | Adding or modifying tests                                                                                 |
| `build`    | Changes to build tools or dependencies                                                                    |
| `ci`       | Continuous integration-related changes                                                                    |
| `chore`    | Maintenance tasks (not affecting app logic or behavior)                                                   |
| `revert`   | Reverts a previous commit                                                                                 |

---

## Scope (Optional)

You can add a scope to clarify what part of the project is affected.

**Examples:**

```
feat(api): add endpoint to rename folders
fix(ui): correct button alignment on mobile
```

**Common scopes:** `api`, `db`, `auth`, `ui`, `file`, `folder`, `validation`, etc.

---

## Body (Optional)

Use the body to describe **what** the change does and **why**. Provide any relevant background, rationale, or links to related issues.

---

## Footer (Optional)

The footer is used for metadata, such as:

```
BREAKING CHANGE: updated file model to include file type

Closes #123
```

---

##  Commit Type Decision Table

| Question                                                             | Commit Type          | If NO         |
| -------------------------------------------------------------------- | -------------------- | ------------- |
| Does this introduce new behavior visible to users (UI/API)?          | `feat`               | Next question |
| Does this fix incorrect behavior (bugs, crashes, validation errors)? | `fix`                | Next question |
| Is this a code improvement without changing behavior?                | `refactor` or `perf` | Next question |
| Is this about testing only?                                          | `test`               | Next question |
| Is this a documentation-only change?                                 | `docs`               | Next question |
| Is this related to build system, CI, or toolchain configuration?     | `build` or `ci`      | Next question |
| Is this a dev task or general maintenance unrelated to app logic?    | `chore`              | Next question |
| Does this undo a previous commit?                                    | `revert`             | Next question |

---

## Best Practices

- ✅ Keep commits small and focused
    
- ✅ Use the body section to explain _why_, not just _what_
    
- ✅ Commit frequently and meaningfully
    
- ❌ Don’t mix unrelated changes in the same commit
    
- ❌ Don’t use vague messages like `update stuff`
    

---

## Links
[Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/) 

---

## Extra
###  What Counts as a Feature?

A `feat` refers to a **user-facing** addition. If it adds **new behavior or functionality** that users can interact with (via the UI, API, CLI, etc.), it qualifies as a feature.

#### Examples:

- Adding a new API route
    
- Introducing form validation with error feedback
    
- Enabling a new permission or access control feature
    

