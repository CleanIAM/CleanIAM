using System.Net;

namespace SharedKernel.Infrastructure;

/// <summary>
/// This class represents the result of an operation.
///
/// It is inspired by Rust's Result type.
/// </summary>
public class Result
{
    internal bool Success { get; set; }
    internal string? ErrorMessage { get; set; }
    internal int? ErrorCode { get; set; }

    public Error ErrorValue => !Success
        ? new Error { Message = ErrorMessage ?? string.Empty, Code = ErrorCode ?? 0 }
        : throw new InvalidOperationException("Result is not an error");

    /// <summary>
    /// Creates a new Result object representing success.
    /// </summary>
    /// <returns>Result object representing success</returns>
    public static Result Ok()
    {
        return new Result { Success = true };
    }

    /// <summary>
    /// Helper function to automatically infer generics type with explicitly specifying it 
    /// </summary>
    /// <param name="value">Value of the success</param>
    /// <typeparam name="T">Type for the generics Result</typeparam>
    /// <returns></returns>
    public static Result<T> Ok<T>(T value) where T : class => Result<T>.Ok(value);

    /// <summary>
    /// Creates a new Result object representing an error without description.
    /// </summary>
    /// <returns>Result object representing an error</returns>
    public static Result Error()
    {
        return new Result { Success = false };
    }

    /// <summary>
    /// Creates a new Result object representing an error.
    /// </summary>
    /// <param name="errorMessage">Error message</param>
    /// <param name="errorCode">Error code</param>
    /// <returns>Result object representing an error</returns>
    public static Result Error(string errorMessage, int errorCode)
    {
        return new Result { Success = false, ErrorMessage = errorMessage, ErrorCode = errorCode };
    }

    /// <summary>
    /// Creates a new Result object representing an error.
    /// </summary>
    /// <param name="errorMessage">Error message</param>
    /// <param name="errorCode">Error code</param>
    /// <returns>Result object representing an error</returns>
    public static Result Error(string errorMessage, HttpStatusCode errorCode)
    {
        return new Result { Success = false, ErrorMessage = errorMessage, ErrorCode = (int)errorCode };
    }


    /// <summary>
    /// Creates a new Result object representing an error.
    /// </summary>
    /// <param name="errorMessage">Error message</param>
    /// <returns>Result object representing an error</returns>
    public static Result Error(string errorMessage)
    {
        return new Result { Success = false, ErrorMessage = errorMessage };
    }

    /// <summary>
    /// Check if the result object represents success.
    /// </summary>
    /// <returns>`true` if object represents success, otherwise `false`</returns>
    public bool IsOk()
    {
        return Success;
    }

    /// <summary>
    /// Check if the result object represents error.
    /// </summary>
    /// <returns>`true` if object represents error, otherwise `false`</returns>
    public bool IsError()
    {
        return !Success;
    }
}

/// <summary>
/// This class represents the result of an operation.
///
/// It is inspired by Rust's Result type.
/// </summary>
public class Result<T> where T : class
{
    private bool Success { get; set; }
    private T? SuccessValue { get; set; }
    private string? ErrorMessage { get; set; }
    private int? ErrorCode { get; set; }


    /// <summary>
    /// This method allows to implicitly convert non-generics error result to generics error result
    /// to make the code cleaner.
    /// </summary>
    /// <param name="result">Non-generics result to convert from</param>
    /// <returns>Converted generics result</returns>
    /// <remarks>
    /// This conversion should only be applied if the result represents error!
    /// If it represents success, then the generics result should be created using `Result.Ok(object)`
    /// that automatically converts to generics error if given some parameter. 
    /// </remarks>
    public static implicit operator Result<T>(Result result) => new()
    {
        Success = result.Success,
        ErrorMessage = result.ErrorMessage,
        ErrorCode = result.ErrorCode,
        SuccessValue = null
    };


    /// <summary>
    /// Value of the result if it represents success.
    /// </summary>
    /// <exception cref="InvalidOperationException">If tried to get Value and result is error </exception>
    public T Value => SuccessValue ?? throw new InvalidOperationException("Result is not a success");


    /// <summary>
    /// Error value of the result if it represents an error.
    /// </summary>
    /// <exception cref="InvalidOperationException">If tried to get Error value and result is ok </exception>
    public Error ErrorValue => !Success
        ? new Error { Message = ErrorMessage ?? string.Empty, Code = ErrorCode ?? 0 }
        : throw new InvalidOperationException("Result is not an error");


    /// <summary>
    /// Creates a new Result object representing success.
    /// </summary>
    /// <returns>Result object representing success</returns>
    public static Result<T> Ok(T value)
    {
        return new Result<T> { Success = true, SuccessValue = value };
    }

    /// <summary>
    /// Creates a new Result object representing an error.
    /// </summary>
    /// <param name="errorMessage">Error message</param>
    /// <param name="errorCode">Error code</param>
    /// <returns>Result object representing an error</returns>
    public static Result<T> Error(string errorMessage, int errorCode)
    {
        return new Result<T> { Success = false, ErrorMessage = errorMessage, ErrorCode = errorCode };
    }


    /// <summary>
    /// Creates a new Result object representing an error.
    /// </summary>
    /// <param name="errorMessage">Error message</param>
    /// <param name="errorCode">Error code</param>
    /// <returns>Result object representing an error</returns>
    public static Result<T> Error(string errorMessage, HttpStatusCode errorCode)
    {
        return new Result<T> { Success = false, ErrorMessage = errorMessage, ErrorCode = (int)errorCode };
    }


    /// <summary>
    /// Creates a new Result object representing an error.
    /// </summary>
    /// <param name="errorMessage">Error message</param>
    /// <returns>Result object representing an error</returns>
    public static Result<T> Error(string errorMessage)
    {
        return new Result<T> { Success = false, ErrorMessage = errorMessage };
    }

    /// <summary>
    /// Check if the result object represents success.
    /// </summary>
    /// <returns>`true` if object represents success, otherwise `false`</returns>
    public bool IsOk()
    {
        return Success;
    }

    /// <summary>
    /// Check if the result object represents error.
    /// </summary>
    /// <returns>`true` if object represents error, otherwise `false`</returns>
    public bool IsError()
    {
        return !Success;
    }
}

public struct Error
{
    public string Message { get; set; }
    public int Code { get; set; }
}