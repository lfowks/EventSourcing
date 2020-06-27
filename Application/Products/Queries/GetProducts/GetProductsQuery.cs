using Application.Common.Interfaces;
using Application.Products.Queries.GetProduct;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Products.Queries.GetProducts
{
    public class GetProductsQuery:IRequest<ProductsVm>
    {
    }

    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, ProductsVm>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetProductsQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ProductsVm> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {

            var products = await _context.Products
                    .OrderByDescending(t => t.Id)
                    .ToListAsync(); //cancellationToken

            return new ProductsVm
            {
                Data = _mapper.Map<List<ProductDto>>(products)
            };
        }
    }
}
