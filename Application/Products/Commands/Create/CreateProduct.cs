using Application.Common.Interfaces;
using Domain.Entities;
using EventStore.ClientAPI;
using EventStore.ClientAPI.Common.Log;
using EventStore.ClientAPI.Projections;
using EventStore.ClientAPI.SystemData;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Products.Commands.Create
{
    public class CreateProduct : IRequest<int>
    {
        public string Name { get; set; }

        public string Description{ get; set; }
    }

    public class CreateTodoItemCommandHandler : IRequestHandler<CreateProduct, int>
    {
        private readonly IApplicationDbContext _context;
        private IEventStoreConnection _eventStore;
        public CreateTodoItemCommandHandler(IApplicationDbContext context, IEventStoreConnection eventStore)//IEventStoreConnection eventStore
        {
            _context = context;
            _eventStore = eventStore;
        }

        public async Task<int> Handle(CreateProduct request, CancellationToken cancellationToken)
        {
            var entity = new Product
            {
                Name = request.Name,
                Description = request.Description,
            };

            entity.CreatedShortDate = DateTime.Now.ToShortDateString();

            _context.Products.Add(entity);

            await _context.SaveChangesAsync();//cancellationToken



            //EventStore();
            // Publish();
            //Event Store Test

            //var conn = EventStoreConnection.Create(new Uri("tcp://admin:changeit@localhost:1113"));

            //var connectionSettings = ConnectionSettings.Create();
            //var conn = EventStoreConnection.Create(
            //  "ConnectTo=tcp://127.0.0.1:1115;DefaultUserCredentials=admin:changeit;UseSslConnection=true;TargetHost=eventstore.org;ValidateServer=false",
            //  connectionSettings, "Task");

            //conn.ConnectAsync().Wait();



            PublishEvent("{\"a\":\"1\"}", JsonConvert.SerializeObject(entity));


            PublishEvent("{\"a\":\"1\"}", JsonConvert.SerializeObject(entity));


            PublishEvent("{\"a\":\"1\"}", JsonConvert.SerializeObject(entity));

            CreateProjection();


            List<Product> products = new List<Product>();

            var readEvents = _eventStore.ReadStreamEventsForwardAsync("$projections-products-counter-result", 0, 10, true).Result;
            foreach (var evt in readEvents.Events)
            {
                string data = Encoding.UTF8.GetString(evt.Event.Data);
                var obj = JObject.Parse(data).SelectToken("event").First.ToString();
                Product product = JsonConvert.DeserializeObject<Product>(obj);
                products.Add(product);
            }




            //var data = Encoding.UTF8.GetBytes("{\"a\":\"2\"}");
            //var metadata = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(entity));
            //var evt = new EventData(Guid.NewGuid(), "testEvent", true, data, metadata);

            //_eventStore.AppendToStreamAsync("test-stream", ExpectedVersion.Any, evt).Wait();

            //var streamEvents = _eventStore.ReadStreamEventsForwardAsync("a_test_stream", 0, 1, false).Result;
            //var returnedEvent = streamEvents.Events[0].Event;



            return entity.Id;
        }

        public void PublishEvent(string data, string metaData)
        {
            _eventStore.AppendToStreamAsync(
                "a_test_stream",
                ExpectedVersion.Any,
                new EventData(
                    Guid.NewGuid(),
                    "ItemAdded",
                    true,
                    Encoding.UTF8.GetBytes(metaData),
                    Encoding.UTF8.GetBytes(data)
                )
            ).Wait();
        }

        public void CreateProjection()
        {

            UserCredentials userCredentials = new UserCredentials("admin", "changeit");

            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            ProjectionsManager projection = new ProjectionsManager(new ConsoleLogger(),
            new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2113), TimeSpan.FromMilliseconds(5000), handler);


            string countItemsProjection = "";
            //countItemsProjection += "options({";
            //countItemsProjection += "resultStreamName: 'error-result',";
            //countItemsProjection += " $includeLinks: false,";
            //countItemsProjection += "reorderEvents: false,";
            //countItemsProjection += " processingLag: 0";
            //countItemsProjection += "})";
            countItemsProjection += "fromAll()";
            countItemsProjection += "   .when({";
            countItemsProjection += "    $init: function(){";
            countItemsProjection += "        return {";
            countItemsProjection += "            event:[]";
            countItemsProjection += "        }";
            countItemsProjection += "    },";
            countItemsProjection += "    ItemAdded: function(s,e){";
            countItemsProjection += "        var date = new Date('" + DateTime.Now.Year + "/"+ DateTime.Now.Month+"/"+ DateTime.Now.Day+ "');";
            countItemsProjection += "        var dateEvent = new Date(Date.parse(e.body.CreatedShortDate));";
            countItemsProjection += "        if(dateEvent <= date){";
            countItemsProjection += "            s.event.push(e.body)";
            countItemsProjection += "    }";
            countItemsProjection += "   }";
            countItemsProjection += "  }).outputState()";

            try
            {

                string projectionName = "errorSnapchot-"+ DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day;
                projection.CreateContinuousAsync(projectionName, countItemsProjection, userCredentials).Wait();//"products-counter", 
                var result = projection.GetResultAsync(projectionName, userCredentials);
                result = projection.GetStatisticsAsync(projectionName, userCredentials);
                projection.DisableAsync(projectionName,userCredentials);
                //projection.DisableAsync("products-counter", userCredentials);
            }
            catch (Exception e)
            {

                throw;
            }
           
        }
    }
}
