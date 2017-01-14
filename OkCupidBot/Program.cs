using OkCupidBot.Common;
using OkCupidBot.Models;
using OkCupidBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForecastIO;
using System.Device.Location;

namespace OkCupidBot
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceManager.Services.Add(new ArgumentService());
            ServiceManager.Services.Add(new LogService());
            ServiceManager.Services.Add(new DatabaseService());
            ServiceManager.Services.Add(new WebService());
            ServiceManager.Services.Add(new MessageService());
            ServiceManager.Services.Add(new DiscordService());
            
            // This is purely for testing purposes to test the messaging system.
            //ServiceManager.Services.WebService.Initialize();
            //ServiceManager.Services.Add(new OkCupidService());
            //ServiceManager.Services.OkCupidService.Login();
            //Profile prof = ServiceManager.Services.OkCupidService.GetProfile("colleen_elizabet", new Uri("https://www.okcupid.com/profile/colleen_elizabet"));
            //string msesage = ServiceManager.Services.MessageService.GetMessage(prof);

            Setup setup = new Setup();
            int exitCode = setup.Execute();
            ServiceManager.Services.LogService.WriteHeader("OkCupidBot Completed");
            ServiceManager.Services.LogService.WriteLine("Exit Code: \"{0}\"", exitCode);
            Console.ReadKey();

            ServiceManager.Services.LogService.WriteHeader("Cleaning Services");
            ServiceManager.Services.Dispose();

            Environment.Exit(exitCode);
        }
    }
}
