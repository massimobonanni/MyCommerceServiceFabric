using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Customer.Interfaces;
using Microsoft.ServiceFabric.Actors;
using MyCommerce.SF.Core.Constants;
using MyCommerce.SF.Core.Interfaces;
using MyCommerce.SF.Core.Utilities;
using WebApi.Dto.Customers;
using WebApi.Requests.Customers;
using WebApi.Responses.Customers;

namespace WebApi.Controllers
{
    [ServiceRequestActionFilter]
    public class CustomersController : ApiController
    {
        private IActorFactory ActorFactory;

        public CustomersController(IActorFactory actorFactory)
        {
            if (actorFactory == null) throw new ArgumentNullException("actorFactory");
            ActorFactory = actorFactory;
        }

        [Route("api/customers/{username}")]
        [HttpGet]
        [ResponseType(typeof(GetCustomerInfoResponse))]
        public async Task<HttpResponseMessage> GetCustomerInfo(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return Request.CreateResponse(HttpStatusCode.InternalServerError);

            HttpResponseMessage response = null;

            var custProxy = ActorFactory.Create<ICustomerActor>(new ActorId(username), ServiceNames.ApplicationName,
                ServiceNames.CustomerServiceName);

            try
            {
                var customerInfo = await custProxy.GetCustomerInfoAsync();
                if (customerInfo != null && customerInfo.IsValid)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new GetCustomerInfoResponse()
                    {
                        Data = new Dto.Customers.CustomerInfoDto(customerInfo),
                        IsSuccess = true
                    });
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }

            return response;
        }

        [Route("api/customers/{username}/ActiveShoppingCart")]
        [HttpPost]
        [ResponseType(typeof(AddProductToActiveShoppingCartResponse))]
        public async Task<HttpResponseMessage> AddProductToActiveShoppingCart(string username,
            [FromBody] AddProductToActiveShoppingCartRequest request)
        {
            if (string.IsNullOrWhiteSpace(username))
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            if (request == null) return Request.CreateResponse(HttpStatusCode.InternalServerError);

            HttpResponseMessage response = null;

            var custProxy = ActorFactory.Create<ICustomerActor>(new ActorId(username), ServiceNames.ApplicationName,
                ServiceNames.CustomerServiceName);

            try
            {
                var addResult = await custProxy.AddProductToShoppingCartAsync(request.Data.ProductId,
                    request.Data.ProductDescription, request.Data.UnitCost, request.Data.Quantity);

                response = Request.CreateResponse(HttpStatusCode.OK, new AddProductToActiveShoppingCartResponse()
                {
                    IsSuccess = addResult
                });
            }
            catch (Exception ex)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
            return response;
        }
    }
}
