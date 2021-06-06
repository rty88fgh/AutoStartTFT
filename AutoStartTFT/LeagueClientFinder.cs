using AutoStartTFT.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoStartTFT
{
    public class LeagueClientFinder
    {
        private const string ProcessName = "LeagueClientUx";
        public event EventHandler Closed;
        public bool IsFound => Process != null;
        public event EventHandler<LeagueClientInfoEventArgs> GetedProcess;
        public string ExecutablePath { get; private set; }
        private Process Process { get; set; }
        
        private CancellationTokenSource _cts;
        private ILogger<LeagueClientFinder> _logger;
        private Task _checkProcess;

        public LeagueClientFinder()
        {
            _cts = new CancellationTokenSource();
            _logger = LoggerFactory.Create(cfg => { })
                .CreateLogger<LeagueClientFinder>();
        }

        public LeagueClientFinder(ILogger<LeagueClientFinder> logger) : this()
        {
            _logger = logger;
        }

        public Task<bool> StartAsync(CancellationToken ct)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
#pragma warning disable CA1416 // Validate platform compatibility
            if (!IsCurrentProcessAdmin())
#pragma warning restore CA1416 // Validate platform compatibility
            {
                var errorMsg = "Current user did not have permission. Please run as Administartor";
                _logger.LogError(errorMsg);
                return Task.FromResult(false);
            }
            _checkProcess = Task.Factory.StartNew(() => WaitForProcessAsync(),
                _cts.Token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);

            return Task.FromResult(true);
        }

        public Task StopAsync() 
        {
            _cts.Cancel();
            _checkProcess.Wait();
            return Task.CompletedTask;
        }

        public Task WaitForProcessAsync()
        {
            while (!_cts.IsCancellationRequested)
            {
                if (Process != null)
                {
                    _cts.Token.WaitHandle.WaitOne(100);
                    continue;
                }

                var processes = Process.GetProcessesByName(ProcessName);
                if (processes.Any())
                {
                    Process = processes.First();
                    Process.EnableRaisingEvents = true;
                    Process.Exited += OnProcessExited;

                    ExecutablePath = Path.GetDirectoryName(Process.MainModule.FileName);
                    _logger.LogInformation("LeaguageClient is found");
                    GetedProcess?.Invoke(this, new LeagueClientInfoEventArgs()
                    {
                        LeagueClientPath = ExecutablePath
                    });
                }

                _cts.Token.WaitHandle.WaitOne(100);
            }

            return Task.CompletedTask;
        }

        private void OnProcessExited(object sender, EventArgs e)
        {
            Process.Exited -= OnProcessExited;
            ExecutablePath = null;
            Process = null;

            Closed?.Invoke(sender, e);
        }

        [SupportedOSPlatform("windows")]
        public bool IsCurrentProcessAdmin()
        {
            using var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            var principal = new System.Security.Principal.WindowsPrincipal(identity);
            return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }

        

        
    }
}
