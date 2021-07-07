using AutoStartTFT.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AutoStartTFT
{
    public class TFTGameWorker
    {
        private LeagueClientInvoker _invoker;
        private LeagueClientFinder _finder;
        private CancellationTokenSource _cts;
        private ILogger<TFTGameWorker> _logger;
        private Task _repeatTask;
        private ILoggerFactory _factory;
        private CancellationTokenSource _waitingToRestartCts;
        private Random _random;
        private bool _isWorking;

        public TFTGameWorker(ILoggerFactory factory)
        {
            _factory = factory;
            _logger = factory.CreateLogger<TFTGameWorker>();
            _random = new Random();
        }

        public Task<bool> StartAsync(CancellationToken ct)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            _repeatTask = Task.Factory.StartNew(() => RepeatTFTGamesAsync(), _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            return Task.FromResult(true);
        }

        private async Task RepeatTFTGamesAsync()
        {
            while (!_cts.IsCancellationRequested)
            {
                _waitingToRestartCts = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token);

                _finder = new LeagueClientFinder(_factory.CreateLogger<LeagueClientFinder>());
                _invoker = new LeagueClientInvoker(_finder, _factory.CreateLogger<LeagueClientInvoker>());


                _logger.LogInformation("Init Invoker...");
                var invokerStartResult = await _invoker.StartAsync(_cts.Token);
                if (!invokerStartResult)
                {
                    _logger.LogError("Failed to start invoker");
                    StopAllProcessAndWaitTenSecToRestart();
                    continue;
                }

                var result = await _finder.StartAsync(_cts.Token);
                if (!result)
                {
                    _logger.LogError("Failed to start watcher");
                    StopAllProcessAndWaitTenSecToRestart();
                    continue;
                }

                _invoker.Event.Subscribe(OnReceiveEvent);

                _logger.LogInformation("Waiting to find LeagueClient...");
                while (!_finder.IsFound && !_waitingToRestartCts.IsCancellationRequested)
                    _waitingToRestartCts.Token.WaitHandle.WaitOne(100);

                if (_waitingToRestartCts.IsCancellationRequested)
                {
                    _logger.LogWarning("Process has been stoped when waiting to find LeagueClient...");
                    StopAllProcessAndWaitTenSecToRestart();
                    break;
                }

                _logger.LogInformation("LeagueClient was found");
                _logger.LogInformation("Starting to waiting invoker ready...");
                while (!_invoker.IsReady && !_waitingToRestartCts.IsCancellationRequested)
                    _waitingToRestartCts.Token.WaitHandle.WaitOne(100);

                if (_waitingToRestartCts.IsCancellationRequested)
                {
                    _logger.LogWarning("Process has been stoped when waiting invoker ready...");
                    StopAllProcessAndWaitTenSecToRestart();
                    break;
                }

                _logger.LogInformation("Invoker started!!!");

                var gameFlowUrl = "/lol-gameflow/v1/gameflow-phase";
                var response = await _invoker.SendRequestAsync(HttpMethodEnum.Get,
                    gameFlowUrl,
                    null,
                    null);

                OnReceiveEvent(new LeagueEvent()
                {
                    Uri = gameFlowUrl,
                    Data = JsonDocument.Parse(response)
                });
                //var request = new LobbyRequest()
                //{
                //    QueueId = (int)GameModeEnum.TFT
                //};
                //var response = await _invoker.SendRequestAsync<LobbyRequest, LobbyResponse>(HttpMethodEnum.Post, "/lol-lobby/v2/lobby", null, request);
                //if (response == null)
                //{
                //    _logger.LogError("Failed to create TFT room");
                //    StopAllProcessAndWaitTenSec();
                //    continue;
                //}

                //_logger.LogInformation("Create TFT room successfully");

                //localCts.Token.WaitHandle.WaitOne(1000);
                //var searchResponse = await _invoker.SendRequestAsync(HttpMethodEnum.Post,
                //    "/lol-lobby/v2/lobby/matchmaking/search",
                //    null,
                //    null);
                //if (searchResponse == null)
                //{
                //    _logger.LogError("Failed to create TFT room");
                //    StopAllProcessAndWaitTenSec();
                //    continue;
                //}

                //_logger.LogInformation("Search team now...");
                _waitingToRestartCts.Token.WaitHandle.WaitOne();
            }
        }

        private async void OnReceiveEvent(LeagueEvent obj)
        {
            _logger.LogInformation("Message:{msg}", obj);
            return;
            //_logger.LogDebug("Receive event:{event}", obj);
            if (obj.Uri != "/lol-gameflow/v1/gameflow-phase")
                return;

            while (_isWorking)
                _waitingToRestartCts.Token.WaitHandle.WaitOne(100);

            if (_waitingToRestartCts.IsCancellationRequested)
                return;

            _isWorking = true;
            var gameStatus = obj.Data.RootElement.GetString();
            _logger.LogInformation("Receive game status is {status}", gameStatus);

            switch (gameStatus)
            {
                case "EndOfGame":
                    await WaitingSec(Program.Settings.MaxWaitingSecWhenFinishGame);
                    await CreateTFTRoom();
                    break;
                case "None":
                    await CreateTFTRoom();
                    break;
                case "Lobby":
                    await SearchTeamsTask();
                    break;
                case "Matchmaking":
                    break;
                case "ReadyCheck":
                    await WaitingSec(Program.Settings.MaxWaitingSecWhenFinishGame);
                    await ReadyToGameTask();
                    break;
                case "ChampSelect":
                    break;
                case "GameStart":
                    break;
                case "InProgress":
                    break;

            }

            _isWorking = false;
        }

        private Task WaitingSec(int maxSec)
        {
            var waitingSec = _random.Next(maxSec);
            _logger.LogInformation("Waiting {waitSec} sec ...", waitingSec);
            return Task.Delay(waitingSec * 1000);
        }

        private async Task SearchTeamsTask()
        {
            var searchResponse = await _invoker.SendRequestAsync(HttpMethodEnum.Post,
                "/lol-lobby/v2/lobby/matchmaking/search",
                null,
                null);
            if (searchResponse == null)
            {
                _logger.LogError("Failed to Search TFT room");
                StopAllProcessAndWaitTenSecToRestart();
            }
        }

        private async Task ReadyToGameTask()
        {
            var searchResponse = await _invoker.SendRequestAsync(HttpMethodEnum.Post,
                "/lol-matchmaking/v1/ready-check/accept",
                null,
                null);
            if (searchResponse == null)
            {
                _logger.LogError("Failed to ready TFT games");
                StopAllProcessAndWaitTenSecToRestart();
            }
        }

        private async Task CreateTFTRoom()
        {
            var request = new LobbyRequest()
            {
                QueueId = (int)GameModeEnum.TFT
            };
            var response = await _invoker.SendRequestAsync<LobbyRequest, LobbyResponse>(HttpMethodEnum.Post, "/lol-lobby/v2/lobby", null, request);
            if (response == null)
            {
                _logger.LogError("Failed to create TFT room");
                StopAllProcessAndWaitTenSecToRestart();
            }
        }

        public async Task StopAsync()
        {
            _cts.Cancel();
            await _finder.StopAsync();
            await _invoker.StopAsync();
        }

        private void StopAllProcessAndWaitTenSecToRestart()
        {
            _logger.LogInformation("Stop all process...");
            _finder?.StopAsync().Wait();
            _invoker?.StopAsync().Wait();
            _logger.LogInformation("Waiting for 10 sec to restart...");
            if (!_cts.IsCancellationRequested)
                _cts.Token.WaitHandle.WaitOne(10000);

            _waitingToRestartCts.Cancel();
        }
    }
}
