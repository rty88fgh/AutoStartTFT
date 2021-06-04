using AutoStartTFT.Model;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AutoStartTFT
{
    class Program
    {
        private static CancellationTokenSource _globalCts;
        private static ILogger<Program> _logger;
        static void Main(string[] args)
        {
            if (!OperatingSystem.IsWindows())
            {
                Console.WriteLine("Please run in windows");
                return;
            }

            _globalCts = new CancellationTokenSource();
            Log.Logger = new LoggerConfiguration()
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
                var processWatcher = new LeagueClientFinder(loggerFactory.CreateLogger<LeagueClientFinder>());
                var localCts = CancellationTokenSource.CreateLinkedTokenSource(_globalCts.Token);
                var invoker = new LeagueClientInvoker(processWatcher, loggerFactory.CreateLogger<LeagueClientInvoker>());

                var invokerStartResult = invoker.StartAsync(localCts.Token).Result;
                if (!invokerStartResult)
                {
                    _logger.LogError("Failed to start invoker");
                    StopAllProcessAndWaitTenSec(processWatcher, invoker);
                    break;
                }


                _logger.LogInformation("Waiting to find LeagueClient...");
                var waitinWatcherCst = CancellationTokenSource.CreateLinkedTokenSource(localCts.Token);
                processWatcher.GetedProcess += (o, e) => waitinWatcherCst.Cancel();

                var result = processWatcher.StartAsync(_globalCts.Token).Result;
                if (!result)
                {
                    _logger.LogError("Failed to start watcher");
                    StopAllProcessAndWaitTenSec(processWatcher, invoker);
                    break;
                }
                waitinWatcherCst.Token.WaitHandle.WaitOne();

                

                
                _logger.LogInformation("Starting to waiting invoker ready...");

                var delay = Task.Delay(200000);
                var checkIsReadyCts = CancellationTokenSource.CreateLinkedTokenSource(localCts.Token);
                var checkIsReadyTask = Task.Factory.StartNew(() =>
                {
                    while (!checkIsReadyCts.IsCancellationRequested)
                    {
                        if (invoker.IsReady)
                            break;
                        checkIsReadyCts.Token.WaitHandle.WaitOne(100);
                    }

                }, checkIsReadyCts.Token);

                var firstTask = Task.WhenAny(delay, checkIsReadyTask).Result;
                if (firstTask == delay)
                {
                    _logger.LogError("Failed to wait invoker ready ");
                    checkIsReadyCts.Cancel();
                    StopAllProcessAndWaitTenSec(processWatcher, invoker);
                    break;
                }

                _logger.LogInformation("Invoker started!!!");

                var request = new LobbyRequest()
                {
                    QueueId = (int)GameModeEnum.TFT
                };
                var response = invoker.SendRequestAsync<LobbyRequest, LobbyResponse>(HttpMethodEnum.Post, "/lol-lobby/v2/lobby", null, request).Result;
                if(response == null)
                {
                    _logger.LogError("Failed to create TFT room");
                    StopAllProcessAndWaitTenSec(processWatcher,invoker);
                    continue;
                }

                _logger.LogInformation("Create TFT room successfully");

                localCts.Token.WaitHandle.WaitOne(1000);
                var searchResponse = invoker.SendRequestAsync(HttpMethodEnum.Post, 
                    "/lol-lobby/v2/lobby/matchmaking/search", 
                    null, 
                    null).Result;
                if (response == null)
                {
                    _logger.LogError("Failed to create TFT room");
                    StopAllProcessAndWaitTenSec(processWatcher, invoker);
                    continue;
                }


                _logger.LogInformation("Search team now...");


                _globalCts.Token.WaitHandle.WaitOne();

            }


            _logger.LogInformation("Please enter any key to exit process...");
            Console.ReadKey();

        }

        private static void StopAllProcessAndWaitTenSec(LeagueClientFinder finder, LeagueClientInvoker invoker)
        {
            _logger.LogInformation("Stop all process...");
            finder?.StopAsync().Wait();
            invoker?.StopAsync().Wait();
            _logger.LogInformation("Waiting for 10 sec to restart...");
            _globalCts.Token.WaitHandle.WaitOne(10000);
        }
    }
}
