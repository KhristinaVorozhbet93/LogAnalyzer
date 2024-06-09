using LogAnalyzer.Domain.Interfaces;

namespace LogAnalyzer.Domain.LogRecord
{
    public interface ILogRepository : IRepositoryEF<LogRecord>
    {
    }
}
