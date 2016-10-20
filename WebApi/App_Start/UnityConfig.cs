using Microsoft.Practices.Unity;
using System.Web.Http;
using MyCommerce.SF.Core.Interfaces;
using MyCommerce.SF.Core.Utilities;
using Unity.WebApi;

namespace WebApi
{
    public static class UnityConfig
    {
        public static UnityContainer RegisterComponents(HttpConfiguration GlobalConfiguration)
        {
            var container = new UnityContainer();

            container.RegisterType<IActorFactory, ReliableFactory>();
            container.RegisterType<IServiceFactory, ReliableFactory>();

            return container;
        }
    }
}