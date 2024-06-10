using LogAnalyzer.Domain.LogRecord;
using Microsoft.Extensions.Options;

namespace LogAnalyzer.WebAPI
{
    public class LogFileProcessor : BackgroundService, ILogFileProcessorService
    {
        private readonly ILogger<LogFileProcessor> _logger;
        private readonly IOptions<LogFileProcessorSettings> _options;
        private readonly IServiceProvider _serviceProvider;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private DateTime _lastStartTime;
        private int _currentFileIndex = 0;
        private int _countFiles = 0;

        //взять из бд
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
            _lastStartTime = DateTime.Now;

            while (!stoppingToken.IsCancellationRequested && !_cancellationTokenSource.IsCancellationRequested)
            {
                var logFiles = Directory.GetFiles(_options.Value.DirectoryPath, "*.log", SearchOption.TopDirectoryOnly);
                _countFiles = logFiles.Length;

                foreach (var logFile in logFiles.OrderBy(f => f).Take(_options.Value.CountFiles))
                {
                    if (!_processedFiles.Contains(logFile))
                    {
                        await ReadAndProcessFileAsync(logFile, stoppingToken);

                        _processedFiles.Add(logFile);
                        _countFiles++;
                        _currentFileIndex++;
                        _logger.LogInformation("Файл обработан: {logFile}", logFile);
                    }
                }
                _currentFileIndex = 0;

                await Task.Delay(TimeSpan.FromSeconds(_options.Value.TimeInterval), stoppingToken);
            }
        }

        private async Task ReadAndProcessFileAsync(string logFile, CancellationToken stoppingToken)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var localServiceProvider = scope.ServiceProvider;
            var logReaderService = localServiceProvider.GetRequiredService<ILogReaderService>();

            await logReaderService.ReadFromFileAsync(logFile, stoppingToken);
            File.Delete(logFile);
            _logger.LogInformation("Файл удален: {logFile}", logFile);
        }


        public LogStatus GetStatus()
        {
            var elapsedTime = DateTime.Now - _lastStartTime;
            var progres = _countFiles > 0 ? (int)Math.Round(((double)_currentFileIndex / _countFiles) * 100) : 0;
            var processedFiles = _currentFileIndex;
            return new LogStatus(progres, processedFiles, _lastStartTime, elapsedTime);
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _logger.LogInformation("Фоновй сервис остановлен");
        }

        public void Start()
        {
            _cancellationTokenSource = new();         
            StartAsync(CancellationToken.None);
            _logger.LogInformation("Фоновй сервис запущен");
        }
    }
}


public class LogStatus
{
    public double Progress { get; set; }
    public int ProcessedFiles { get; set; }
    public DateTime LastStartTime { get; set; }
    public TimeSpan ElapsedTime { get; set; }
    public LogStatus(double progress, int processedFiles, DateTime lastStartTime, TimeSpan elapsedTime)
    {
        Progress = progress;
        ProcessedFiles = processedFiles;
        LastStartTime = lastStartTime;
        ElapsedTime = elapsedTime;
    }
}


public interface ILogFileProcessorService
{
    LogStatus GetStatus();
    public void Stop();
    public void Start();

}



