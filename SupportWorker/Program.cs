using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using System.Configuration;

namespace SupportWorker
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
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
