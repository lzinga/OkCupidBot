using OkCupidBot.Models;
using OkCupidBot.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OkCupidBot.Common
{
    public static class Load
    {
        public static ProfileSettings ProfileSettings()
        {
            string fileName = ServiceManager.Services.ArgumentService.Arguments.ProfileSettingsFileName;
            string fullpath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Config", fileName);

            return System.IO.File.ReadAllText(fullpath).Deserialize<ProfileSettings>();
        }
    }
}
