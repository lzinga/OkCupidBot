using OkCupidBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OkCupidBot
{
    class Program
    {

        static void Main(string[] args)
        {
            Setup setup = new Setup();
            int exitCode = setup.Execute();
            ServiceManager.Services.LogService.WriteHeader("OkCupidBot Completed");
            ServiceManager.Services.LogService.WriteLine("Exit Code: \"{0}\"", exitCode);

            ServiceManager.Services.LogService.WriteHeader("Cleaning Services");
            ServiceManager.Services.Dispose();

            Environment.Exit(exitCode);
        }
    }
}
