namespace FashionEcommerce.API.Models;

public class ApiResponse<T>
{
    public string Status { get; set; } = "success"; // Default to success
    public T? Data { get; set; }
    public string? Message { get; set; }
    public string? Code { get; set; }

    // Fix: Add optional 'message' parameter to Success
    public static ApiResponse<T> Success(T data, string? message = null) 
        => new() { Status = "success", Data = data, Message = message };

    public static ApiResponse<T> Fail(T? data, string? message = null) 
        => new() { Status = "fail", Data = data, Message = message };

    public static ApiResponse<T> Error(string message, string? code = null) 
        => new() { Status = "error", Message = message, Code = code };
}
