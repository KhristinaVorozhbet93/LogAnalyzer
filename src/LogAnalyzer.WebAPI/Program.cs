using Minio;
using Microsoft.EntityFrameworkCore;
using LogsAnalyzer.Infrastructure.DataEntityFramework.Repositories;
using LogsAnalyzer.Infrastructure.DataEntityFramework;
using LogAnalyzer.Infrastructure.LogFilterService;
using LogAnalyzer.Infrastructure.LogFileReaderService;
using LogAnalyzer.Domain.Interfaces;
using LogAnalyzer.Domain.LogRecord;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;


namespace LogAnalyzer.WebAPI
{

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var postgresConfig = builder.Configuration
                .GetRequiredSection("PostgresConfig")
                .Get<PostgresConfig>();
            if (postgresConfig is null)
            {
                throw new InvalidOperationException("PostgresConfig is not configured");
            }

            var minioClientConfig = builder.Configuration
                .GetRequiredSection("MinioClientSettings")
                .Get<MinioClientSettings>();

            if (minioClientConfig is null)
            {
                throw new InvalidOperationException("MinioClientConfig is not configured");
            }

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(
                    $"Host={postgresConfig.ServerName};" +
                    $"Port={postgresConfig.Port};" +
                    $"Database={postgresConfig.DatabaseName};" +
                    $"Username={postgresConfig.UserName};" +
                    $"Password={postgresConfig.Password};"));

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddOptions<IPConfiguration>()
                .BindConfiguration("IPConfiguration")
                .ValidateDataAnnotations()
                .ValidateOnStart();

            builder.Services.AddOptions<LogFileProcessorSettings>()
                .BindConfiguration("LogFileProccesorSettings")
                .ValidateDataAnnotations()
                .ValidateOnStart();

            builder.Services.AddOptions<LogMinioProcessorSettings>()
               .BindConfiguration("LogMinioProccesorSettings")
               .ValidateDataAnnotations()
               .ValidateOnStart();

            builder.Services.AddScoped<ILogFilterService, LogFilterService>();
            builder.Services.AddScoped<ILogReaderService, LogFileReaderService>();
            builder.Services.AddScoped(typeof(IRepositoryEF<>), typeof(EFRepository<>));
            builder.Services.AddScoped<ILogRepository, LogRepository>();
            builder.Services.AddMinio(options =>
            {
                options.WithEndpoint(minioClientConfig.Endpoint)
                .WithCredentials(minioClientConfig.AccesKey, minioClientConfig.SecretKey)
                .WithSSL(false)
                .Build();
            });
            

            builder.Services.AddSingleton<LogFileProcessor>();
            builder.Services.AddSingleton<ILogFileProcessorService>(
                sp => sp.GetRequiredService<LogFileProcessor>());
            builder.Services.AddHostedService(sp => sp.GetRequiredService<LogFileProcessor>());

            //builder.Services.AddHostedService<LogMinioProcessor>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

        
            app.MapGet("/get_status", async ([FromServices] LogFileProcessor logService) =>
            {
                var status = logService.GetStatus();
                Console.WriteLine($"Прогресс: {status.Progress}; Осталось файлов: {status.ProcessedFiles}");
            });

            app.MapPost("/stop_handler", async ([FromServices] LogFileProcessor logService) =>
            {
                logService.Stop();
              
            });

            app.MapPost("/start_handler", async ([FromServices] LogFileProcessor logService) =>
            {
                logService.Start();

            });

            app.Run();
        }
    }
}






