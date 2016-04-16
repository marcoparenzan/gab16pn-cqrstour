using SupportService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportSqlAzure
{
    public class SupportSqlAzureService: ISupportService
    {
        public string Operator { get; set; }

        private SupportContext _context;

        protected SupportContext Context
        {
            get
            {
                if (_context == null)
                {
                    _context = new SupportContext(
                        ConfigurationManager.ConnectionStrings["SqlAzure"].ConnectionString
                    );
                }
                return _context;
            }
        }

        protected Database Database
        {
            get
            {
                return Context.Database;
            }
        }

        async Task<IEnumerable<OpenSupportDto>> ISupportService.GetOpenSupportsAsync()
        {
            return await Context.OpenSupports.ToListAsync();
        }

        async Task ISupportService.NewOpenSupportAsync(Guid supportId, string customerName, string customerEMail, string details, DateTimeOffset openDate)
        {
            Context.OpenSupports.Add(new OpenSupportDto
            {
                SupportId = supportId,
                CustomerName = customerName,
                CustomerEMail = customerEMail,
                Details = details,
                OpenDate = openDate
            });
            await Context.SaveChangesAsync();
        }

        async Task ISupportService.HandleAsync(CloseSupport closeSupport)
        {
            throw new NotImplementedException();
        }
    }
}
