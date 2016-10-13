using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyCommerce.Common.Entities;

namespace MyCommerce.DataAccess.Services
{
    public class OrdersService : EntityRepositoryBase<Order, int>
    {
        public OrdersService(bool traceOff = false) : base(traceOff)
        {
        }

        public OrdersService(string nameOrConnectionString, bool traceOff = false) : base(nameOrConnectionString, traceOff)
        {
        }

        public override Task<Order> GetSingleAsNoTrackingAsync(int key)
        {
            throw new NotImplementedException();
        }
    }
}
