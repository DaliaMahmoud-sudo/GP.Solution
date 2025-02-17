using AutoMapper;
using GP.APIs.DTOs;

using GP.Core.Entites.OrderAggregate;
using GP.Core.Entities;

namespace GP.APIs.Helpers
{
    public class MappingProfiles : Profile 
    {
        public MappingProfiles() {

            CreateMap<Product, ProductToReturnDto>()
                    .ForMember(d => d.ImageUrl, O => O.MapFrom<ProductImageUrlResolver>());

        
            }
    } 
}
