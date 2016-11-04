using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyCommerce.Common.Entities;

namespace MyCommerce.DataAccess.Services
{
    public class OrderProductsService : EntityRepositoryBase<OrderProduct, int>
    {
        public OrderProductsService(bool traceOff = false) : base(traceOff)
        {
        }

        public OrderProductsService(string nameOrConnectionString, bool traceOff = false) : base(nameOrConnectionString, traceOff)
        {
        }

        public override Task<OrderProduct> GetSingleAsNoTrackingAsync(int key)
        {
            throw new NotImplementedException();
        }
    }
}
