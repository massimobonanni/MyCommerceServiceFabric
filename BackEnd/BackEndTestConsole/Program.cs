using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Customer.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using MyCommerce.DataAccess.Services;
using MyCommerce.SF.Core.Constants;

namespace BackEndTestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var customerService =
                new CustomersService(
                    "data source=SURFACE-MB;initial catalog=MyCommerce;User Id=sa;Password=sqlServer2016;");

            var proxy = ActorProxy.Create<ICustomerActor>(new ActorId("massimo.bonanni"), ServiceNames.ApplicationName, ServiceNames.CustomerServiceName);

            for (int i = 0; i < 10; i++)
            {
                var info1 = proxy.GetCustomerInfoAsync().GetAwaiter().GetResult();
                Console.WriteLine($"[{i}] From Actor -> {info1.LastName } {info1.FirstName } [{info1.IsEnabled}]");
                Thread.Sleep(20000);
                var infoDb = customerService.GetSingleAsync("massimo.bonanni").GetAwaiter().GetResult();
                Console.WriteLine($"[{i}] From DB -> {infoDb.LastName } {infoDb.FirstName } [{infoDb.IsEnabled}]");
                proxy.EnableCustomerAsync(!infoDb.IsEnabled).GetAwaiter().GetResult();
            }
            Console.ReadLine();
        }
    }
}
