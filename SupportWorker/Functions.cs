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
using SupportService;
using SupportSqlAzure;

namespace PhoneCallWorker
{
    public class Functions
    {
        private static IPhoneCallReader _reader;

        protected static IPhoneCallReader Reader
        {
            get
            {
                if (_reader == null)
                {
                    _reader = new PhoneCallDocumentDbReader();
                }
                return _reader;
            }
        }


        private static ISupportService _service;

        protected static ISupportService Service
        {
            get
            {
                if (_service == null)
                {
                    _service = new SupportSqlAzureService();
                }
                return _service;
            }
        }

        public async static void Handle([ServiceBusTrigger("support", "phonecallevents")] PhoneCallEvent e)
        {
            switch (e.Type)
            {
                case PhoneCallEventType.PhoneCallCreated:
                    var phoneCallDto = await Reader.GetSupportForPhoneCall(e.PhoneCallId);
                    Service.NewOpenSupportAsync(phoneCallDto.PhoneCallId, phoneCallDto.CustomerInfo.Name, phoneCallDto.CustomerInfo.EMail, phoneCallDto.Details, phoneCallDto.OpenDate);
                    break;
                default:
                    throw new NotSupportedException("PhoneCallEvent not recognized");
            }
        }
    }
}
