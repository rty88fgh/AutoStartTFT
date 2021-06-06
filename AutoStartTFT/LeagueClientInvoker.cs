using AutoStartTFT.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AutoStartTFT
{
    public class LeagueClientInvoker
    {
        private LeagueClientFinder _finder;
        private ILogger<LeagueClientInvoker> _logger;
        private CancellationTokenSource _cts;
        private UserAndPortInfo _userInfo;
        private HttpClient _client;
        private Subject<LeagueEvent> _eventSubject;
        private ClientWebSocket _socketClient;
        private CancellationToken _socketClientToken;
        private Task _receiveEventTask;
        private const int ClientEventDataArrayAddress = 2;
        private const int ClientEventNumber = 8;
        public bool IsReady { get; private set; }

        public IObservable<LeagueEvent> Event => _eventSubject.AsObservable();

        public LeagueClientInvoker(LeagueClientFinder finder, ILogger<LeagueClientInvoker> logger)
        {
            _finder = finder;
            _logger = logger;
            _eventSubject = new Subject<LeagueEvent>();
        }

        public Task<bool> StartAsync(CancellationToken ct)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);

            if (_finder == null)
            {
                _logger.LogError("Please enter LeagueClinetFinder.");
                return Task.FromResult(false);
            }

            _finder.GetedProcess += OnGetedProcess;
            _finder.Closed += OnProcessClose;

            if (_finder.IsFound)
                OnGetedProcess(null, new LeagueClientInfoEventArgs());

            return Task.FromResult(true);
        }

        public Task StopAsync()
        {
            _cts.Cancel();
            return Task.CompletedTask;
        }

        private void OnProcessClose(object sender, EventArgs e)
        {
            IsReady = false;
            _userInfo = null;
            _client = null;
        }

        private void OnGetedProcess(object sender, LeagueClientInfoEventArgs e)
        {
            var worker = new LockFileWorker();
            var info = worker.GetUserAndPortInfo(_finder.ExecutablePath).Result;

            if (info == null)
            {
                _logger.LogError("Failed to get user info");
                return;
            }

            _userInfo = info;
            SetupClientAndSocketClient();
            IsReady = true;
        }

        private void SetupClientAndSocketClient()
        {
            var handler = new HttpClientHandler()
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (response, cert, chain, errors) => true,
            };
            _client = new HttpClient(handler);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _userInfo.AuthToken);
            _client.BaseAddress = new Uri($"https://127.0.0.1:{_userInfo.Port}/");
            _socketClient = new ClientWebSocket
            {
                Options =
                    {
                        Credentials = new NetworkCredential("riot", _userInfo.Password),
                        RemoteCertificateValidationCallback =
                            (sender, cert, chain, sslPolicyErrors) => true,
                    }
            };

            _socketClient.Options.AddSubProtocol("wamp");
            _socketClientToken = _cts.Token;
            _receiveEventTask = Task.Factory.StartNew(async () =>
            {
                try
                {
                    await _socketClient.ConnectAsync(new Uri($"wss://127.0.0.1:{_userInfo.Port}/"), _socketClientToken);
                    await _socketClient.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes("[5, \"OnJsonApiEvent\"]")),
                        WebSocketMessageType.Binary,
                        true,
                        _socketClientToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while connecting socket to send request");
                    return;
                }

                while (!_socketClientToken.IsCancellationRequested)
                {
                    var allBytes = new List<byte>();
                    LeagueEvent clientEvent = null;
                    try
                    {
                        while (!_socketClientToken.IsCancellationRequested)
                        {
                            var byteArray = new byte[1024];
                            var buffer = new ArraySegment<byte>(byteArray);
                            await _socketClient.ReceiveAsync(buffer, _socketClientToken);

                            allBytes.AddRange(buffer.Array.Where(arr => arr != '\0'));

                            if (allBytes.Where(b => b == '[').Count() == allBytes.Where(b => b == ']').Count())
                                break;
                        }

                        if (_socketClientToken.IsCancellationRequested)
                            return;

                        var str = Encoding.UTF8.GetString(allBytes.ToArray());
                        if (!string.IsNullOrEmpty(str))
                        {
                            var obj = JsonSerializer.Deserialize<object[]>(str);
                            if (!int.TryParse(obj[0].ToString(), out var eventNum) ||
                               eventNum != ClientEventNumber)
                            {
                                _logger.LogDebug("Reveiving non client event.Str:{str}", str);
                                allBytes.Clear();
                                continue;
                            }

                            clientEvent = JsonSerializer.Deserialize<LeagueEvent>(obj[ClientEventDataArrayAddress].ToString());

                        }


                        _eventSubject.OnNext(clientEvent);

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error while receiving message from socket");
                        break;
                    }
                }
            },
            _socketClientToken,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default);

        }

        public Task<TResponse> SendRequestAsync<TRequest, TResponse>(HttpMethodEnum method, string relativeUrl, IEnumerable<string> queryParameters, TRequest body)
        {
            string str = null;
            if (body != null)
                str = JsonSerializer.Serialize(body, new JsonSerializerOptions()
                {
                    IgnoreNullValues = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

            return SendRequestAsync<TResponse>(method, relativeUrl, queryParameters, str);
        }

        public async Task<string> SendRequestAsync(HttpMethodEnum method,
            string relativeUrl,
            IEnumerable<string> queryParameters,
            string body)
        {
            var url = queryParameters == null
               ? relativeUrl
               : relativeUrl + BuildQueryParameterString(queryParameters);
            var request = new HttpRequestMessage(new HttpMethod(method.ToString()), url);

            if (!string.IsNullOrWhiteSpace(body))
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            try
            {
                var response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while send request");
                return null;
            }
        }

        public async Task<TResponse> SendRequestAsync<TResponse>(HttpMethodEnum method,
            string relativeUrl,
            IEnumerable<string> queryParameters,
            string body)
        {
            var responseJson = await SendRequestAsync(method, relativeUrl, queryParameters, body);
            if (string.IsNullOrEmpty(responseJson) || string.IsNullOrWhiteSpace(responseJson))
                return default(TResponse);

            return JsonSerializer.Deserialize<TResponse>(responseJson);
        }



        protected string BuildQueryParameterString(IEnumerable<string> queryParameters)
        {
            return "?" + string.Join("&", queryParameters.Where(a => !string.IsNullOrWhiteSpace(a)));
        }
    }
}
