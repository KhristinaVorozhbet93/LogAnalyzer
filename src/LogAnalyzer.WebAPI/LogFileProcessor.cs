using LogAnalyzer.Domain.LogRecord;
using Microsoft.Extensions.Options;

namespace LogAnalyzer.WebAPI
{
    public class LogFileProcessor : BackgroundService
    {
        private readonly ILogger<LogFileProcessor> _logger;
        private readonly IOptions<LogFileProcessorSettings> _options;
        private readonly IServiceProvider _serviceProvider;
        private readonly HashSet<string> _processedFiles = new HashSet<string>();

        public LogFileProcessor(ILogger<LogFileProcessor> logger,
            IOptions<LogFileProcessorSettings> options,
            IServiceProvider serviceProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var logFiles = Directory.GetFiles(_options.Value.DirectoryPath, "*.log", SearchOption.TopDirectoryOnly);

                    foreach (var logFile in logFiles.OrderBy(f => f).Take(_options.Value.CountFiles))
                    {
                        if (!_processedFiles.Contains(logFile))
                        {
                            await ReadAndProcessFileAsync(logFile, stoppingToken);
                            _processedFiles.Add(logFile);
                            _logger.LogInformation("Файл обработан: {logFile}", logFile);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при обработке файлов логов");
                }

                await Task.Delay(TimeSpan.FromSeconds(_options.Value.TimeInterval), stoppingToken);
            }
        }

        private async Task ReadAndProcessFileAsync(string logFile, CancellationToken stoppingToken)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var localServiceProvider = scope.ServiceProvider;
            var logReaderService = localServiceProvider.GetRequiredService<ILogReaderService>();
            try
            {
                await logReaderService.ReadFromFileAsync(logFile, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при чтении файла: {logFile}", logFile);
            }
        }
    }
}

