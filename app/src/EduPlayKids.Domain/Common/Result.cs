namespace EduPlayKids.Domain.Common;

/// <summary>
/// Represents the result of an operation with success/failure state and optional error message.
/// Provides a clean way to handle operation results without throwing exceptions for expected failures.
/// </summary>
/// <typeparam name="T">The type of the result value.</typeparam>
public class Result<T>
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the result value if the operation was successful.
    /// </summary>
    public T? Value { get; private set; }

    /// <summary>
    /// Gets the error message if the operation failed.
    /// </summary>
    public string? Error { get; private set; }

    /// <summary>
    /// Initializes a new instance of the Result class.
    /// </summary>
    /// <param name="isSuccess">Whether the operation was successful.</param>
    /// <param name="value">The result value.</param>
    /// <param name="error">The error message.</param>
    private Result(bool isSuccess, T? value, string? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    /// <summary>
    /// Creates a successful result with a value.
    /// </summary>
    /// <param name="value">The result value.</param>
    /// <param name="message">Optional success message.</param>
    /// <returns>A successful result.</returns>
    public static Result<T> Success(T value, string? message = null)
    {
        return new Result<T>(true, value, message);
    }

    /// <summary>
    /// Creates a failed result with an error message.
    /// </summary>
    /// <param name="error">The error message.</param>
    /// <returns>A failed result.</returns>
    public static Result<T> Failure(string error)
    {
        return new Result<T>(false, default(T), error);
    }

    /// <summary>
    /// Implicitly converts a value to a successful result.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator Result<T>(T value)
    {
        return Success(value);
    }

    /// <summary>
    /// Implicitly converts a string to a failed result.
    /// </summary>
    /// <param name="error">The error message.</param>
    public static implicit operator Result<T>(string error)
    {
        return Failure(error);
    }

    /// <summary>
    /// Returns a string representation of the result.
    /// </summary>
    /// <returns>String representation.</returns>
    public override string ToString()
    {
        if (IsSuccess)
        {
            return $"Success: {Value}";
        }
        return $"Failure: {Error}";
    }
}

/// <summary>
/// Non-generic result class for operations that don't return a value.
/// </summary>
public class Result
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the error message if the operation failed.
    /// </summary>
    public string? Error { get; private set; }

    /// <summary>
    /// Initializes a new instance of the Result class.
    /// </summary>
    /// <param name="isSuccess">Whether the operation was successful.</param>
    /// <param name="error">The error message.</param>
    private Result(bool isSuccess, string? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <param name="message">Optional success message.</param>
    /// <returns>A successful result.</returns>
    public static Result Success(string? message = null)
    {
        return new Result(true, message);
    }

    /// <summary>
    /// Creates a failed result with an error message.
    /// </summary>
    /// <param name="error">The error message.</param>
    /// <returns>A failed result.</returns>
    public static Result Failure(string error)
    {
        return new Result(false, error);
    }

    /// <summary>
    /// Implicitly converts a string to a failed result.
    /// </summary>
    /// <param name="error">The error message.</param>
    public static implicit operator Result(string error)
    {
        return Failure(error);
    }

    /// <summary>
    /// Returns a string representation of the result.
    /// </summary>
    /// <returns>String representation.</returns>
    public override string ToString()
    {
        if (IsSuccess)
        {
            return "Success";
        }
        return $"Failure: {Error}";
    }
}