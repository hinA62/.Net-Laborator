using AutoMapper;
using ProductManagement.Features.Product;

namespace ProductManagement.Mapper;

public class BrandInitialsResolver : IValueResolver<Product, ProductProfileDto, string>
{
    public string Resolve(Product source, ProductProfileDto dest, string destMember, ResolutionContext context)
    {
        if (string.IsNullOrWhiteSpace(source.Brand))
            return "?";

        var parts = source.Brand.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 1)
            return parts[0][0].ToString().ToUpper();

        return (parts.First()[0].ToString() + parts.Last()[0]).ToUpper();
    }
}
