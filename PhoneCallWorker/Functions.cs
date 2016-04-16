using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using PhoneCallService;
using PhoneCallDocumentDb;
using Newtonsoft.Json;

namespace PhoneCallWorker
{
    public class Functions
    {
        private static IPhoneCallService _service;

        protected static IPhoneCallService Service
        {
            get
            {
                if (_service == null)
                {
                    _service = new PhoneCallDocumentDbService();
                    _service.Operator = "PhoneCallWorker";
                }
                return _service;
            }
        }

        public async static void Handle([ServiceBusTrigger("phonecallmessages")] PhoneCallMessage message, [ServiceBus("phonecallevents")] ICollector<PhoneCallEvent> events)
        {
            switch (message.Type)
            {
                case PhoneCallMessageType.NewPhoneCall:
                    var phoneCallId = await Service.HandleAsync(message.NewPhoneCall);
                    events.Add(new PhoneCallEvent
                    {
                        PhoneCallId = phoneCallId
                    });
                    break;
                case PhoneCallMessageType.ClosePhoneCall:
                    await Service.HandleAsync(message.ClosePhoneCall);
                    break;
                default:
                    throw new NotSupportedException("PhoneCallMessage not recognized");
            }
        }
    }
}
