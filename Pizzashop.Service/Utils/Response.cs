namespace Pizzashop.Service.Utils;

public class Response<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public T? Data { get; set; }

    public static Response<T> SuccessResponse(T? data = default, string message = "Operation Successful")
    {
        return new Response<T> { Success = true, Data = data, Message = message };
    }

    public static Response<T> FailureResponse(string message = "Internal Server Error")
    {
        return new Response<T> { Success = false, Message = message };
    }
}
