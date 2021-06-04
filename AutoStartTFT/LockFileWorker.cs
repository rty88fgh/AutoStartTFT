using AutoStartTFT.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoStartTFT
{
    public class LockFileWorker
    {
        private readonly ILogger<LockFileWorker> _logger;

        public LockFileWorker()
        {
            _logger = LoggerFactory.Create(cfg => { })
                .CreateLogger<LockFileWorker>();
        }

        public LockFileWorker(ILogger<LockFileWorker> logger) : this()
        {
            _logger = logger;
        }

        public async Task<UserAndPortInfo> GetUserAndPortInfo(string leagueClientPath)
        {
            var path = Path.Combine(leagueClientPath, "lockfile");
            if (!File.Exists(path))
            {
                _logger.LogError("{path} is not exists", path);
                return null;
            }

            try
            {
                var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var butter = new byte[stream.Length];
                await stream.ReadAsync(butter,0, butter.Length);
                var fileStr = Encoding.UTF8.GetString(butter);
                
                var lockFileToken = fileStr.Split(':');
                if (lockFileToken.Length < 4)
                {
                    _logger.LogError("Failed to parse lock file");
                    return null;
                }
                if (!int.TryParse(lockFileToken[2], out var port))
                {
                    _logger.LogError("Failed to parse port. PortString:{str}", lockFileToken[2]);
                    return null;
                }
                return new UserAndPortInfo()
                {
                    Pid = lockFileToken[1],
                    Port = port,
                    Password = lockFileToken[3]
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while parsing lock file");
                return null;
            }

        }
    }
}
