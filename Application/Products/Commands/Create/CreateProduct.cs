using Application.Common.Interfaces;
using Application.Events;
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

        public decimal Price { get; set; }
    }

    public class CreateTodoItemCommandHandler : IRequestHandler<CreateProduct, int>
    {
        private readonly IApplicationDbContext _context;
        private IEventStoreService _eventStoreService;
        public CreateTodoItemCommandHandler(IApplicationDbContext context, IEventStoreService eventStoreService)//IEventStoreConnection eventStore
        {
            _context = context;
            _eventStoreService = eventStoreService;
        }

        public async Task<int> Handle(CreateProduct request, CancellationToken cancellationToken)
        {
            var entity = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
            };

            entity.CreatedShortDate = DateTime.Now.Year+"/"+ DateTime.Now.Month+"/"+DateTime.Now.Day;

            _context.Products.Add(entity);

            await _context.SaveChangesAsync();//cancellationToken

            //Event Aggregate
            _eventStoreService.PublishEvent("{}", JsonConvert.SerializeObject(entity),"ProductAdded", "a_test_stream");

            return entity.Id;
        }

    }
}
