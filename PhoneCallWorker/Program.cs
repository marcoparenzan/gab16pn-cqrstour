using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using System.Configuration;

namespace PhoneCallWorker
{
    class Program
    {
        static void Main()
        {
            var serviceBusConfiguration = new Microsoft.Azure.WebJobs.ServiceBus.ServiceBusConfiguration
            {
                ConnectionString = ConfigurationManager.AppSettings["ServiceBusMessagesConnectionString"]
            };
            var configuration = new JobHostConfiguration();
            configuration.UseServiceBus(serviceBusConfiguration);
            var host = new JobHost(configuration);
            host.RunAndBlock();
        }
    }
}
