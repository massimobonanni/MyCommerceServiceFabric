using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyCommerce.Common.Entities;

namespace MyCommerce.DataAccess.Services
{
    public class ProductsService : EntityRepositoryBase<Product, int>
    {
        public ProductsService(bool traceOff = false) : base(traceOff)
        {
        }

        public ProductsService(string nameOrConnectionString, bool traceOff = false) : base(nameOrConnectionString, traceOff)
        {
        }

        public override Task<Product> GetSingleAsNoTrackingAsync(int key)
        {
            throw new NotImplementedException();
        }
    }
}
