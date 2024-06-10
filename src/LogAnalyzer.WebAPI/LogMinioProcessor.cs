using LogAnalyzer.Domain.LogRecord;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using System.Reactive.Linq;

namespace LogAnalyzer.WebAPI
{
    public class LogMinioProcessor : BackgroundService
    {
        private readonly ILogger<LogMinioProcessor> _logger;
        private readonly IOptions<LogMinioProcessorSettings> _options;
        private readonly IMinioClient _minioClient;
        private readonly IServiceProvider _serviceProvider;


        //переделать на запись в бд
        private readonly HashSet<string> _processedFiles = new HashSet<string>();

        public LogMinioProcessor(ILogger<LogMinioProcessor> logger,
            IOptions<LogMinioProcessorSettings> options,
            IMinioClient minioClient,
            IServiceProvider serviceProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _minioClient = minioClient ?? throw new ArgumentNullException(nameof(minioClient));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await PutFilesInDirectoryAndDeleteFromBucket(stoppingToken);

                var logFiles = Directory.GetFiles(_options.Value.Directory, "*.log", SearchOption.TopDirectoryOnly);

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

            await Task.Delay(TimeSpan.FromSeconds(_options.Value.TimeInterval), stoppingToken);
        }

       


        private async Task PutFilesInDirectoryAndDeleteFromBucket(CancellationToken stoppingToken)
        {
            var bucketArgs = new BucketExistsArgs()
                            .WithBucket(_options.Value.BucketName);
            var listArgs = new ListObjectsArgs()
                  .WithBucket(_options.Value.BucketName);
            if (await _minioClient.BucketExistsAsync(bucketArgs, stoppingToken))
            {
                var logs = _minioClient.ListObjectsAsync(listArgs, stoppingToken);

                foreach (var logFile in logs)
                {
                    string path = Path.Combine(_options.Value.Directory, logFile.Key);

                    var getObjectArgs = new GetObjectArgs()
                        .WithBucket(_options.Value.BucketName)
                        .WithObject(logFile.Key)
                        .WithFile(path);
                    await _minioClient.GetObjectAsync(getObjectArgs, stoppingToken);

                    _logger.LogInformation("Файл скачан: {logFile}", logFile.Key);

                    var removeArgs = new RemoveObjectArgs().WithBucket(_options.Value.BucketName).WithObject(logFile.Key);
                    await _minioClient.RemoveObjectAsync(removeArgs, stoppingToken);

                    _logger.LogInformation("Файл удален: {logFile}", logFile.Key);
                }
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

