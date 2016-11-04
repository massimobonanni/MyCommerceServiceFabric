using Microsoft.ServiceFabric.Actors.Runtime;
using MyCommerce.SF.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer
{
    internal static class CommandFactory
    {
        public static Command CreateCommandForUpdateCustomer(Actor actor, CustomerInfo customer)
        {
            return new Command(actor,
                "[dbo].[Customer_Update]",
                new Dictionary<string, object>() {
                    { "@userName",actor.Id.ToString()},
                    { "@firstName",customer.FirstName },
                    { "@lastName",customer.LastName},
                    { "@isEnabled",customer.IsEnabled},
                });
        }
    }
}
