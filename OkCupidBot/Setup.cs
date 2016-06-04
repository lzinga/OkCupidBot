using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OkCupidBot.Services;
using System.Reflection;
using OkCupidBot.Common;

namespace OkCupidBot
{
    public class Setup
    {
        public Setup()
        {
            if (ServiceManager.Services.ArgumentService.Arguments.Debug)
            {
                ServiceManager.Services.LogService.WriteLine("Debug mode is enabled, after every action it will require input.");
            }
            ServiceManager.Services.LogService.WriteHeader("Starting OkCupidBot");

            foreach (PropertyInfo info in typeof(Options).GetProperties())
            {
                if(info.GetCustomAttribute<SecureAttribute>() != null)
                {
                    int length = ((string)info.GetValue(ServiceManager.Services.ArgumentService.Arguments)).Length;

                    StringBuilder secure = new StringBuilder();
                    secure.Append('*', length);

                    ServiceManager.Services.LogService.WriteLine("{0} = \"{1}\"", info.Name, secure.ToString());
                }
                else
                {
                    ServiceManager.Services.LogService.WriteLine("{0} = \"{1}\"", info.Name, info.GetValue(ServiceManager.Services.ArgumentService.Arguments));
                }
            }

            ServiceManager.Services.WebService.Initialize();
        }

        public int Execute()
        {
            ServiceManager.Services.Add(new OkCupidService());

            // Login to the okcupid profile.
            ServiceManager.Services.OkCupidService.Login();

            // Scan profiles.
            List<Profile> matches =  ServiceManager.Services.OkCupidService.ScanMatches(true);

            return 0;
        }

    }
}
