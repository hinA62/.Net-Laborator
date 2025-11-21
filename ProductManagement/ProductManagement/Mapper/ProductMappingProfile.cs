using AutoMapper;
using ProductManagement.Features.Product;

namespace ProductManagement.Mapper;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<CreateProductProfileRequest, Product>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.ReleaseDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsAvailable,
                opt => opt.MapFrom(src => src.StockQuantity > 0));

        CreateMap<Product, ProductProfileDto>();
    }
}

