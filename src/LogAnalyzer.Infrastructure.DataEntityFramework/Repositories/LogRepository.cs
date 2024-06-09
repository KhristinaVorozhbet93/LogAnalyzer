using LogAnalyzer.Domain.LogRecord;

namespace LogsAnalyzer.Infrastructure.DataEntityFramework.Repositories
{
    public class LogRepository : EFRepository<LogRecord>, ILogRepository
    {
        public LogRepository(AppDbContext appDbContext) : base(appDbContext) { }
    }
}
