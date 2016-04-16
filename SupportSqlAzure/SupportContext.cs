using SupportService;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportSqlAzure
{
    public class SupportContext: DbContext
    {
        public DbSet<OpenSupportDto> OpenSupports { get; set; }

        public SupportContext(string connectionString) : base(connectionString) { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OpenSupportDto>().HasKey(xx => xx.SupportId).ToTable("dbo.OpenSupports");
        }
    }
}
