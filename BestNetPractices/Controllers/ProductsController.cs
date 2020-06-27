using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Products.Commands.Create;
using Application.Products.Commands.Update;
using Application.Products.Commands.Delete;
using Application.Products.Queries.GetProducts;
using Application.Products.Queries.GetProduct;

namespace BestNetPractices.Controllers
{
    //[Authorize]
    [Route("[controller]")]
    public class ProductsController : ApiController
    {

        [HttpGet]
        public async Task<ActionResult<ProductsVm>> Get()
        {
            
            return await Mediator.Send(new GetProductsQuery());
        }

        [HttpGet("{id}")]
        public async Task<ProductVm> Get(int id)
        {
            var vm = await Mediator.Send(new GetProductQuery { Id = id });

            return vm;
        }
        [HttpPost]
        public async Task<ActionResult<int>> Create(CreateProduct command)
        {
            return await Mediator.Send(command);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, UpdateProduct command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }

            await Mediator.Send(command);

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await Mediator.Send(new DeleteProduct { Id = id });

            return NoContent();
        }
    }
}