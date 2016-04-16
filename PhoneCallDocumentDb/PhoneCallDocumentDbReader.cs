using AutoMapper;
using Microsoft.Azure.Documents.Client;
using PhoneCallService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneCallDocumentDb
{
    public class PhoneCallDocumentDbReader: IPhoneCallReader
    {
        public string Operator { get; set; }

        private DocumentClient _client;

        protected DocumentClient Client
        {
            get
            {
                if (_client == null)
                {
                    _client = new DocumentClient(
                        new Uri(ConfigurationManager.AppSettings["DocumentDbEndPoint"])
                        , ConfigurationManager.AppSettings["DocumentDbAccountKey"]
                    );
                }
                return _client;
            }
        }

        async Task<PhoneCallSupportDto> IPhoneCallReader.GetSupportForPhoneCall(Guid phoneCallId)
        {
            var sql = "SELECT pc.id AS PhoneCallId, pc.Operator, pc.OpenDate, pc.Customer, s.Details FROM pc JOIN s in pc.Support WHERE pc.id = '" + phoneCallId +"'";
            var query = Client.CreateDocumentQuery<PhoneCallSupportDto>(UriFactory.CreateDocumentCollectionUri("cqrstour", "CRM"), sql);
            var all = query.ToArray();
            return all[0];
        }
    }
}
