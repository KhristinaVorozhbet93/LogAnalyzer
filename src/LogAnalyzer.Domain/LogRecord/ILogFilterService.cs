using System.Net;

namespace LogAnalyzer.Domain.LogRecord
{
    public interface ILogFilterService
    {
        List<LogRecord> GetIPAddressesWithConfigurations(List<LogRecord> ipAddresses);
        List<LogRecord> GetRangeIPAddresses(List<LogRecord> ipAddresses, IPAddress addressStart, IPAddress addressMask);
        List<LogRecord> GetIPAddressesInTimeInterval(List<LogRecord> ipAddresses, DateTime timeStart, DateTime timeEnd);
        List<LogRecord> GetIPAddressesWithCountTimeRequests(List<LogRecord> ipAddresses);
    }
}
