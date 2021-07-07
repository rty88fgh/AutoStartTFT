using AutoStartTFT.Model;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AutoStartTFT
{
    class Program
    {
        private static CancellationTokenSource _globalCts;
        private static ILogger<Program> _logger;
        public static AutoTFTSettings Settings;
        static void Main(string[] args)
        {
            if (!OperatingSystem.IsWindows())
            {
                Console.WriteLine("Please run in windows");
                return;
            }
            _globalCts = new CancellationTokenSource();
            Settings = new AutoTFTSettings();
            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.txt");
            if (File.Exists(configPath))
            {
                var configStr = File.ReadAllText(configPath);
                try
                {
                    Settings = JsonSerializer.Deserialize<AutoTFTSettings>(configStr);
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "Error while get deserialize settings");
                }
            }

            //logger
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "log.txt"), Serilog.Events.LogEventLevel.Debug, rollingInterval: RollingInterval.Day)
                .CreateLogger();

            var loggerFactory = LoggerFactory.Create((cfg) =>
            {
                cfg.AddSerilog(Log.Logger);
            });
            _logger = loggerFactory.CreateLogger<Program>();


            while (!_globalCts.IsCancellationRequested)
            {
                var worker = new TFTGameWorker(loggerFactory);
                var result = worker.StartAsync(_globalCts.Token).Result;
                if (!result)
                {
                    _logger.LogError("Failed to start worker");
                    break;
                }

                //_globalCts.Token.WaitHandle.WaitOne();
                _logger.LogInformation("Please enter exit to exit process...");
                var exitStr = Console.ReadLine();
                if (exitStr?.ToLower() == "exit")
                    _globalCts.Cancel();
            }


            //_logger.LogInformation("Please enter any key to exit process...");

        }

    }
}
