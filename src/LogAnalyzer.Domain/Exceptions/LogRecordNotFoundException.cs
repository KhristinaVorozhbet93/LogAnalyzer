namespace LogAnalyzer.Domain.Exceptions
{
    public class LogRecordNotFoundException : DomainException
    {
        public LogRecordNotFoundException()
        {
        }

        public LogRecordNotFoundException(string? message) : base(message)
        {
        }

        public LogRecordNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
