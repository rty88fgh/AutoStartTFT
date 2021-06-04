using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoStartTFT.Model
{
    public class UserAndPortInfo
    {
        public string Pid { get; set; }
        public string Password { get; set; }
        public int Port { get; set; } = -1;
        public string AuthToken
        {
            get
            {
                if (string.IsNullOrEmpty(Password) || Port == -1)
                    return null;

                var headerStr = "riot:" + Password;
                return Convert.ToBase64String(Encoding.ASCII.GetBytes(headerStr));
            }
        }
    }
}
