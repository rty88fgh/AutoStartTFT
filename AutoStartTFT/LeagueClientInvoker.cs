using AutoStartTFT.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
        public bool IsReady { get; private set; }

        public LeagueClientInvoker(LeagueClientFinder finder, ILogger<LeagueClientInvoker> logger)
        {
            _finder = finder;
            _logger = logger;
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
            var info = worker.GetUserAndPortInfo(e.LeagueClientPath).Result;

            if (info == null)
            {
                _logger.LogError("Failed to get user info");
                return;
            }

            _userInfo = info;
            SetupClient();
            IsReady = true;
        }

        private void SetupClient()
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
