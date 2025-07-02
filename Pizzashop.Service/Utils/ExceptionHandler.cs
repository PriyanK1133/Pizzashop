using Pizzashop.Entity.Constants;

namespace Pizzashop.Service.Utils;
public static class ExceptionHandler
{
    public static async Task<Response<T>> HandleExceptionsAsync<T>(Func<Task<Response<T>>> func)
    {
        try
        {
            return await func();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return Response<T>.FailureResponse(MessageConstants.DefaultErrorMessage);
        }
    }   
}
