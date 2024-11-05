namespace VPKFILEPROCESSOR.SHARED.Services.ResponseService
{
    /// <summary>
    /// Response Service class is created to return the response data after processing the file.It a generic class that can be used to return any type of data.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="message"></param>
    /// <param name="isSuccess"></param>
    public class ResponseService<T>(T data, string message, bool isSuccess)
    {
        public T? Data { get; set; } = data;
        public string Message { get; set; } = message;
        public bool IsSuccess { get; set; } = isSuccess;
    }
}