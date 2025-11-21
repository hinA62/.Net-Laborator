using AutoMapper;
using ProductManagement.Features.Product;

namespace ProductManagement.Mapper;

public class PriceFormatterResolver : IValueResolver<Product, ProductProfileDto, string>
{
    public string Resolve(Product source, ProductProfileDto dest, string destMember, ResolutionContext context)
    {
        return source.Price.ToString("C2");
    }
}
