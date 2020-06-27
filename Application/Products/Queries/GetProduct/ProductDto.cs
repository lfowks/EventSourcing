using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Products.Queries.GetProduct
{
    public class ProductDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Product, ProductDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => (int)s.Id));
            profile.CreateMap<Product, ProductDto>()
                .ForMember(d => d.Name, opt => opt.MapFrom(s => (string)s.Name));
            profile.CreateMap<Product, ProductDto>()
                .ForMember(d => d.Description, opt => opt.MapFrom(s => (string)s.Description));
        }
    }
}
