using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyCommerce.Common.Entities;
using System.Data.Entity;

namespace MyCommerce.DataAccess.Services
{
    public class CustomersService : EntityRepositoryBase<Customer, string>
    {
        public CustomersService(bool traceOff = false) : base(traceOff)
        {
        }

        public CustomersService(string nameOrConnectionString, bool traceOff = false) : base(nameOrConnectionString, traceOff)
        {
        }

        public override Task<Customer> GetSingleAsNoTrackingAsync(string key)
        {
            return this.Query().FirstOrDefaultAsync(a => a.Username == key);
        }
    }
}
