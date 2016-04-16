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
    public class PhoneCallDocumentDbService: IPhoneCallService
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

        private MapperConfiguration _mapperConfiguration;
        private IMapper _mapper;

        protected IMapper Mapper
        {
            get
            {
                if (_mapper == null)
                {
                    _mapperConfiguration = new MapperConfiguration(cfg => {
                        cfg.CreateMap<NewPhoneCall, PhoneCall>();
                        cfg.CreateMap<CustomerInfo, Customer>();
                        cfg.CreateMap<NewProduct, ProductRequest>();
                        cfg.CreateMap<NewSupport, SupportRequest>();
                    });
                    _mapper = _mapperConfiguration.CreateMapper();
                }
                return _mapper;
            }
        }

        protected T1 Map<T1>(object target)
        {
            return Mapper.Map<T1>(target);
        }

        async Task<Guid> IPhoneCallService.HandleAsync(NewPhoneCall newPhoneCall)
        {
            var phoneCall = new PhoneCall {
                Customer = Map<Customer>(newPhoneCall.CustomerInfo)
                , Operator = Operator
                , Products = Map<ProductRequest[]>(newPhoneCall.Products)
                , Support = Map<SupportRequest[]>(newPhoneCall.Support)
            };
            var documentCollectionLink = UriFactory.CreateDocumentCollectionUri("cqrstour", "CRM");
            var response = await Client.CreateDocumentAsync(documentCollectionLink, phoneCall);

            return Guid.Parse(response.Resource.Id);
        }

        async Task IPhoneCallService.HandleAsync(ClosePhoneCall closePhoneCall)
        {
            var sql = "SELECT * FROM crm WHERE crm.id = '" + closePhoneCall.PhoneCallId + "'";
            var query = Client.CreateDocumentQuery<PhoneCall>(UriFactory.CreateDocumentCollectionUri("cqrstour", "CRM"), sql);
            var all = query.ToArray();
            var phoneCall = all[0];
            phoneCall.EndDate = DateTime.Now;
            var documentCollectionLink = UriFactory.CreateDocumentCollectionUri("cqrstour", "CRM");
            await Client.UpsertDocumentAsync(documentCollectionLink, phoneCall);
        }
    }
}
