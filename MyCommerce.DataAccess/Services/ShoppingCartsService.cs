using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyCommerce.Common.Entities;

namespace MyCommerce.DataAccess.Services
{
    public class ShoppingCartsService : EntityRepositoryBase<ShoppingCart, int>
    {
        public ShoppingCartsService(bool traceOff = false) : base(traceOff)
        {
        }

        public ShoppingCartsService(string nameOrConnectionString, bool traceOff = false) : base(nameOrConnectionString, traceOff)
        {
        }

        public override Task<ShoppingCart> GetSingleAsNoTrackingAsync(int key)
        {
            throw new NotImplementedException();
        }
    }
}
