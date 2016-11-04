using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyCommerce.Common.Entities;

namespace MyCommerce.DataAccess.Services
{
    public class ShoppingCartProductsService : EntityRepositoryBase<ShoppingCartProduct, int>
    {
        public ShoppingCartProductsService(bool traceOff = false) : base(traceOff)
        {
        }

        public ShoppingCartProductsService(string nameOrConnectionString, bool traceOff = false) : base(nameOrConnectionString, traceOff)
        {
        }

        public override Task<ShoppingCartProduct> GetSingleAsNoTrackingAsync(int key)
        {
            throw new NotImplementedException();
        }
    }
}
