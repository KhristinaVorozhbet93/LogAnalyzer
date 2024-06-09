using LogAnalyzer.Domain.LogRecord;
using System.Globalization;
using System.Net;

namespace LogAnalyzer.Infrastructure.LogFileReaderService
{
    public class LogFileReaderService : ILogReaderService
    {
        //возможно, фильтрация пригодится позже
        private readonly ILogFilterService _logFilterService;
        private readonly ILogRepository _logRepository;
        public LogFileReaderService(ILogFilterService logFilterService,
            ILogRepository logRepository)
        {
            _logFilterService = logFilterService ?? throw new ArgumentNullException(nameof(logFilterService));
            _logRepository = logRepository ?? throw new ArgumentNullException(nameof(logRepository));
        }
        public async Task ReadFromFileAsync(string filePath, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException
                    ($"Файл по заданному пути не обнаружен: {filePath}");
            }

            List<LogRecord> logs = new List<LogRecord>();
            CultureInfo provider = new CultureInfo("ru-RU");
            using (StreamReader reader = new StreamReader(filePath))
            {
                string? line;
                while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        var parts = line.Split(':', StringSplitOptions.RemoveEmptyEntries);
                        var requestTime =
                            DateTime.ParseExact($"{parts[0]} {parts[1]}:{parts[2]}:{parts[3]}", "yyyy-MM-dd HH:mm:ss", provider);
                        var applicationName = parts[4].Trim();
                        var stage = parts[5].Trim();
                        var ipAddress = IPAddress.Parse(parts[6].Trim());
                        var clientName = parts[7].Trim();
                        var clientVersion = parts[8].Trim();
                        var path = parts[9].Trim();
                        var method = parts[10].Trim();
                        var statusCode = parts[11].Trim();
                        var statusMessage = parts[12].Trim();
                        var contentType = parts[13].Trim();
                        var contentLength = Convert.ToInt32(parts[14].Trim());
                        var executionTime = TimeSpan.Parse(parts[15].Trim());
                        var memoryUsage = Convert.ToInt32(parts[16].Trim());

                        var logRecord = new LogRecord
                        (Guid.NewGuid(), requestTime, applicationName, stage, ipAddress, clientName, clientVersion, path, method,
                        statusCode, statusMessage, contentType, contentLength, executionTime, memoryUsage);

                        //await _logRepository.Add(logRecord, cancellationToken);
                    }
                }
            }
        }
    }
}