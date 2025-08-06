namespace MyApplication.Exceptions;

public class AppErrorException : ApplicationException
{
    public AppErrorException(string message) : base(message)
    {
    }
}