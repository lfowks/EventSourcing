using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Events;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BestNetPractices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        IEventStoreService _eventStoreService;
        public EventsController(IEventStoreService eventStoreService)
        {
            _eventStoreService = eventStoreService;
        }
        // GET: api/<EventsController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<EventsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<EventsController>
        [HttpPost]
        public void Post([FromBody] CartItem cartItem)
        {
            cartItem.CreatedShortDate = DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day;
            _eventStoreService.PublishEvent("{}", JsonConvert.SerializeObject(cartItem),"CartItemAdded","cart-stream");
        }

        // POST api/<EventsController>
        [HttpPost]
        [Route("api/cancel")]
        public void Cancel([FromBody] CartItem cartItem)
        {
            _eventStoreService.CreateProjection("cancelCart", "CartItemAdded");
        }

        [HttpGet]
        [Route("api/getCartSnapshot")]
        public List<CartItem> GetCartFromSnapshot(string projectionName)
        {
            return _eventStoreService.ReadCartEvents(projectionName);
        }

        //// PUT api/<EventsController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<EventsController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
