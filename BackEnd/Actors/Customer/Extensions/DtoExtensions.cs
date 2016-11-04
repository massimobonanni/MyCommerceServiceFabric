using Customer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Extensions
{
    internal static class DtoExtensions
    {

        internal static CustomerInfoDto AsCustomerInfoDto(this CustomerInfo customer)
        {
            return new CustomerInfoDto()
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                IsEnabled = customer.IsEnabled,
                IsValid = customer.IsValid
            };
        }

        internal static ShoppingCartInfoDto AsShoppingCartInfoDto(this ShoppingCartInfo cart)
        {
            return new ShoppingCartInfoDto()
            {
                Reference = cart.Reference
            };
        }
    }
}
