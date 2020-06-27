using Application.Common.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Products.Queries.GetProduct
{
    public class GetProductQuery: IRequest<ProductVm>
    {
        public int Id { get; set; }
    }

    public class GetProductQueryHandler : IRequestHandler<GetProductQuery, ProductVm>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetProductQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ProductVm> Handle(GetProductQuery request, CancellationToken cancellationToken)
        {
            var productFound = await _context.Products
                .Where(p => p.Id == request.Id)
                   .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
                   .ToListAsync();

            return _mapper.Map<ProductVm>(productFound.FirstOrDefault());
        }
    }
}
