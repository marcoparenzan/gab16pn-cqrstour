using Microsoft.ServiceBus.Messaging;
using PhoneCallService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneCallServiceBus
{
    public class PhoneCallServiceBusService : IPhoneCallService
    {
        public string Operator { get; set; }

        private QueueClient _client;

        protected QueueClient Client
        {
            get
            {
                if (_client == null)
                {
                    _client = QueueClient.CreateFromConnectionString(
                        ConfigurationManager.AppSettings["ServiceBusMessagesConnectionString"]
                    );
                }
                return _client;
            }
        }

        async Task<Guid> IPhoneCallService.HandleAsync(NewPhoneCall newPhoneCall)
        {
            var envelope = new PhoneCallMessage {
                Sender = Operator,
                Type = PhoneCallMessageType.NewPhoneCall,
                NewPhoneCall = newPhoneCall
            };
            var message = new BrokeredMessage(envelope);
            await Client.SendAsync(message);

            return Guid.NewGuid();
        }

        async Task IPhoneCallService.HandleAsync(ClosePhoneCall closePhoneCall)
        {
            var envelope = new PhoneCallMessage
            {
                Sender = Operator,
                Type = PhoneCallMessageType.ClosePhoneCall,
                ClosePhoneCall = closePhoneCall
            };
            var message = new BrokeredMessage(envelope)
            {
            };
            await Client.SendAsync(message);
        }
    }
}
