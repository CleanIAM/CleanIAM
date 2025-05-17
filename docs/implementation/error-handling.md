# Error Handling Pattern

The CleanIAM project implements a custom error handling pattern that avoids traditional exception-based error flow in favor of a more predictable, functional approach. As described in Section 6.5 of the thesis, this approach uses a dedicated `Result` type to represent both successful and error outcomes.

## Result Type

The `Result` type is a custom implementation inspired by the Result pattern from functional programming languages like Rust. It provides a structured way to handle operation outcomes without resorting to exceptions for control flow.

### Structure and Features

The `Result` type has two variants:
1. **Success**: Represents a successful operation, optionally with a payload
2. **Error**: Represents a failed operation with error details

Key features of the `Result` type include:

- **Generic payload support**: Success results can carry typed data using `Result<T>`
- **Error message handling**: Error results include descriptive messages
- **HTTP status code integration**: Error results can include HTTP status codes
- **ASP.NET Core integration**: The `Result` class implements `IActionResult` for direct use in controllers

### Example Usage

Here's how the `Result` type is typically used in command handlers:

```csharp
public static async Task<Result<UserDto>> HandleAsync(CreateUserCommand command, 
    IUserService userService)
{
    // Validation logic
    if (string.IsNullOrEmpty(command.Username))
        return Result.Error("Username cannot be empty", 400);
    
    // Business logic
    var user = await userService.CreateUserAsync(command.Username, command.Email);
    
    // Success with payload
    return Result.Ok(user.Adapt<UserDto>());
}
```

## HTTP Response Integration

When a `Result` is returned from an API controller, it automatically converts to the appropriate HTTP response:

- For success results with no data (`Result`): Returns HTTP 204 No Content
- For success results with data (`Result<T>`): Returns HTTP 200 OK with the data serialized to JSON
- For error results: Returns the specified status code (or 500 Internal Server Error by default) with an error object

The error response has the following format:

```json
{
  "Message": "The message describing the error",
  "Code": 400
}
```

## Benefits

This error handling approach offers several advantages:

1. **Explicit error handling**: Developers must explicitly check for errors, reducing the likelihood of unhandled error cases
2. **Predictable control flow**: No hidden exception paths makes code flow more predictable
3. **Consistent error responses**: API clients receive consistent, well-formatted error information
4. **Reduced overhead**: Avoiding exceptions for normal control flow improves performance
5. **Clear intent**: The code clearly communicates possible failure modes through return types

By using the `Result` type throughout the application, CleanIAM achieves a consistent, maintainable approach to error handling that aligns well with modern API design practices.
