using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkCupidBot.Services
{
    public class LogService : BaseService
    {
        private ServiceManager Service = ServiceManager.Services;

        private void WriteDate()
        {
            Console.Write("{0}: ", DateTime.Now.ToString());
        }

        public void WriteLine(string str, params object[] args)
        {
            WriteDate();
            Console.Write(str, args);
            Console.Write(Environment.NewLine);
        }


        public void WriteHeader(string str, params object[] args)
        {
            if (ServiceManager.Services.ArgumentService.Arguments.Debug)
            {
                WriteLine("Press any key to continue...");
                Console.ReadLine();
            }

            WriteLine("======================================================================");
            WriteLine(str, args);
            WriteLine("======================================================================");
        }

        public void WriteSubHeader(string str, params object[] args)
        {
            WriteLine("-----------------------------------");
            WriteLine(str, args);
            WriteLine("-----------------------------------");
        }

        public void WriteLine(string str, ConsoleColor color, params object[] args)
        {
            // Write the date as gray.
            Console.ForegroundColor = ConsoleColor.Gray;
            WriteDate();
            Console.ResetColor();

            // Write the data as the color.
            Console.ForegroundColor = color;
            Console.Write(str, args);
            Console.ResetColor();

            // Place a new line to ensure only 1 item on each line.
            Console.Write(Environment.NewLine);
        }

        public void Dispose()
        {
            // nothing.
        }
    }
}
