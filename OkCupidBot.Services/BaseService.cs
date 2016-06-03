using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkCupidBot.Services
{
    public class BaseService : IService
    {
        public ServiceManager Services = ServiceManager.Services;


        public void Dispose()
        {
            
        }
    }
}
