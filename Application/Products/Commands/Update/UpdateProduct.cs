using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Products.Commands.Update
{
    public class UpdateProduct : IRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }
    }

    public class UpdateTodoItemCommandHandler : IRequestHandler<UpdateProduct>
    {
        private readonly IApplicationDbContext _context;

        public UpdateTodoItemCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateProduct request, CancellationToken cancellationToken)
        {
            var entity = await _context.Products.FindAsync(request.Id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Product), request.Id);
            }

            entity.Name = request.Name;
            entity.Description = request.Description;

            await _context.SaveChangesAsync();//cancellationToken

            return Unit.Value;
        }
    }
}
