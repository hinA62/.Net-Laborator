using ProductManagement.Features.Product;

namespace ProductManagement.Mapper;

using AutoMapper;

public class AdvancedProductMappingProfile : Profile
{
    public AdvancedProductMappingProfile()
    {
        CreateMap<CreateProductProfileRequest, Product>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.ReleaseDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsAvailable,
                opt => opt.MapFrom(src => src.StockQuantity > 0));

        CreateMap<Product, ProductProfileDto>()
            .ForMember(dest => dest.PriceFormatted,
                opt => opt.MapFrom<PriceFormatterResolver>())
            .ForMember(dest => dest.CategoryDisplay,
                opt => opt.MapFrom<CategoryDisplayResolver>())
            .ForMember(dest => dest.ProductAge,
                opt => opt.MapFrom<ProductAgeResolver>())
            .ForMember(dest => dest.BrandInitials,
                opt => opt.MapFrom<BrandInitialsResolver>())
            .ForMember(dest => dest.AvailabilityStatus,
                opt => opt.MapFrom<AvailabilityStatusResolver>())

            .ForMember(dest => dest.ImageUrl, opt =>
                opt.MapFrom(src =>
                    src.Category == ProductCategory.Home ? null : src.ImageUrl))
            .ForMember(dest => dest.Price, opt =>
                opt.MapFrom(src =>
                    src.Category == ProductCategory.Home ? src.Price * 0.9m : src.Price));

    }
}
