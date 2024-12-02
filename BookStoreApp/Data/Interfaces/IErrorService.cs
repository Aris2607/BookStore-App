namespace BookStoreApp.Data.Interfaces
{
    public interface IErrorService
    {
        Task Error(int code, string message, string stacktrace);
    }
}
