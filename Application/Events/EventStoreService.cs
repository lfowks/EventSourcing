using Domain.Entities;
using EventStore.ClientAPI;
using EventStore.ClientAPI.Common.Log;
using EventStore.ClientAPI.Projections;
using EventStore.ClientAPI.SystemData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Application.Events
{
    public class EventStoreService: IEventStoreService
    {
        private IEventStoreConnection _eventStore;
        public EventStoreService(IEventStoreConnection eventStore)
        {
            _eventStore = eventStore;
        }
        public void PublishEvent(string metaData, string data, string eventType,string stream)
        {
            _eventStore.AppendToStreamAsync(
                stream,
                ExpectedVersion.Any,
                new EventData(
                    Guid.NewGuid(),
                    eventType,
                    true,
                    Encoding.UTF8.GetBytes(data),
                    Encoding.UTF8.GetBytes(metaData)
                )
            ).Wait();
        }

        public void CreateProjection(string name, string eventType)
        {

            UserCredentials userCredentials = new UserCredentials("admin", "changeit");

            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            ProjectionsManager projection = new ProjectionsManager(new ConsoleLogger(),
            new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2113), TimeSpan.FromMilliseconds(5000), handler);

            string projectionName = name + "-" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day;

            string countItemsProjection = "";
            //countItemsProjection += "options({";
            //countItemsProjection += "resultStreamName: '"+ projectionName + "'";
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
            countItemsProjection += "    "+ eventType + ": function(s,e){";
            countItemsProjection += "        var date = new Date('" + DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day + "');";
            countItemsProjection += "        var dateEvent = new Date(Date.parse(e.body.CreatedShortDate));";
            countItemsProjection += "        if(dateEvent <= date){";
            countItemsProjection += "            s.event.push(e.body.CartItems)";
            countItemsProjection += "    }";
            countItemsProjection += "   }";
            countItemsProjection += "  }).outputState()";

            try
            {

                projection.CreateContinuousAsync(projectionName, countItemsProjection, userCredentials).Wait();//"products-counter", 
                var result = projection.GetResultAsync(projectionName, userCredentials);
                // result = projection.GetStatisticsAsync(projectionName, userCredentials);
                projection.DisableAsync(projectionName, userCredentials);
            }
            catch (Exception e)
            {

                throw;
            }

        }

        public Object ReadEvents(string projectionName)
        {
            var readEvents = _eventStore.ReadStreamEventsForwardAsync(projectionName, 0, 10, true).Result;
            Object objectDesarialized = new Object();
            foreach (var evt in readEvents.Events)
            {
                string data = Encoding.UTF8.GetString(evt.Event.Data);
                objectDesarialized = JsonConvert.DeserializeObject<Object>(data);
             
            }

            return objectDesarialized;

        }

        public ICollection<CartItem> ReadCartEvents(string projectionName)
        {
            var readEvents = _eventStore.ReadStreamEventsForwardAsync(projectionName, 0, 10, true).Result;

            Cart cart = new Cart();

            foreach (var evt in readEvents.Events)
            {
                string data = Encoding.UTF8.GetString(evt.Event.Data);
                var obj = JObject.Parse(data).SelectToken("event").First.ToString();
                cart.CartItems = JsonConvert.DeserializeObject<List<CartItem>>(obj);
                //list.Add(product);
            }

            return cart.CartItems;

        }
    }
}
