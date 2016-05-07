using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using System.Text;
using Microsoft.Practices.ServiceLocation;
 

namespace CQRS.NET.Controllers
{
    [RoutePrefix("api")]
    public class WebApiController : ApiController
    {
        ICommandBus _bus = null;
        IQueryDispatcher _queryDispatcher = null;

        public WebApiController()
            : this(ServiceLocator.Current.TryGet<ICommandBus>(), 
                ServiceLocator.Current.TryGet<IQueryDispatcher>())
        {

        }

        public WebApiController(ICommandBus bus, IQueryDispatcher queryDispatcher)
        {
            _bus = bus;
            _queryDispatcher = queryDispatcher;
        }

        [Route("query/{name:alpha}")]
        [HttpPost]
        public HttpResponseMessage Query(string name, [FromBody] dynamic dataTransferObject, 
            [FromUri] QueryOptions options)
        {
            IQueryMessage query = Mapper.Map<IQueryMessage>(name, ToDynamic(dataTransferObject));

            Result r = _queryDispatcher.Dispatch<IQueryMessage>(query, GetCurrentContext(options));

            return ToQueryResponse(r, options);
        }

        [Route("command/{name:alpha}")]
        [HttpPost]
        public HttpResponseMessage Command(string name, [FromBody] dynamic dataTransferObject, 
            [FromUri] CommandOptions options)
        {
            ICommandMessage cmd = Mapper.Map<ICommandMessage>(name, ToDynamic(dataTransferObject));

            Result r = _bus.Send(cmd, GetCurrentContext(options));

            return ToResponse(r, options);
        }

        private dynamic ToDynamic(object expando)
        {
            if (expando == null)
                return null;

            if (expando.GetType() == typeof(Newtonsoft.Json.Linq.JObject))
            {
                dynamic d = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Dynamic.ExpandoObject>(expando.ToString());

                return d;
                // return Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(expando.ToString());

                //return System.Web.Helpers.Json.Decode(System.Web.Helpers.Json.Encode(expando));
            }
            else
            {
                var properties = expando as IDictionary<string, object>;

                if (properties == null)
                    return null;

                dynamic d = expando;

                return d;
            }
        }

        private HttpResponseMessage ToQueryResponse(Result r, QueryOptions options)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            response.StatusCode = HttpStatusCode.BadRequest;

            if (r != null)
            {
                if (r.Success)
                    response.StatusCode = HttpStatusCode.OK;

                var output = options.WithWrap ? r : r.Data;

                response.Content = new StringContent(System.Web.Helpers.Json.Encode(output), Encoding.UTF8, "application/json");

            }

            return response;
        }

        private HttpResponseMessage ToResponse(Result r, OperationOptions options)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            response.StatusCode = HttpStatusCode.BadRequest;

            if (r != null)
            {
                if (r.Success)
                    response.StatusCode = HttpStatusCode.OK;

                if (options.HasFormat() && options.Format.ToLower() == "json")
                    response.Content = new StringContent(System.Web.Helpers.Json.Encode(r), Encoding.UTF8, "application/json");
                else
                    response.Content = new StringContent(r.ObjectId, Encoding.UTF8, "text/csv");
 
            }

            return response;
        }

        private InvocationContext GetCurrentContext(OperationOptions options)
        {
            InvocationContext context = new InvocationContext();

            context.Options = options;
            context.ParticipantId = User.Identity.GetUserId();

            return context;
        }
    }
}
