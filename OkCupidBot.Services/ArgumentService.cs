using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Security;
using OkCupidBot.Common;

namespace OkCupidBot.Services
{
    public class Options
    {
        public bool Debug { get; set; }

        public string Username { get; set; }

        [Secure]
        public string Password { get; set; }
    }

    public class ArgumentService : BaseService
    {
        private string[] _args;
        private Options _options;

        public ArgumentService()
        {
            _args = Environment.GetCommandLineArgs();
            this.ParseArguments();
        }

        public Options Arguments
        {
            get
            {
                return _options;
            }
        }

        public void Dispose()
        {
            // nothing
        }

        private void ParseArguments()
        {
            _options = new Options();
            if (_args.Length > 1)
            {
                for(int i = 1; i < _args.Length; i++)
                {
                    string full = _args[i];

                    if (full.Contains("="))
                    {
                        string key = full.Split('=')[0].Replace("/", "").Replace("\\", "").Trim();
                        string value = full.Split('=')[1].Trim();

                        PropertyInfo info = typeof(Options).GetProperty(key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                        info.SetValue(_options, value);
                    }
                    else
                    {
                        string key = full.Replace("/", "").Replace("\\", "").Trim();

                        PropertyInfo info = typeof(Options).GetProperty(key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                        info.SetValue(_options, true);
                    }

                }
            }
        }


    }
}
