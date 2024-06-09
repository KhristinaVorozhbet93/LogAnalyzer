using LogAnalyzer.Domain.LogRecord;
using Microsoft.EntityFrameworkCore;

namespace LogsAnalyzer.Infrastructure.DataEntityFramework
{
    public class AppDbContext : DbContext
    {
        DbSet<LogRecord> Logs => Set<LogRecord>();
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

    }
}
