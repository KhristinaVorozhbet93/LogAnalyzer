namespace LogAnalyzer.Domain.LogRecord
{
    public interface ILogReaderService
    {
        Task ReadFromFileAsync(string path, CancellationToken cancellationToken);
    }
}
