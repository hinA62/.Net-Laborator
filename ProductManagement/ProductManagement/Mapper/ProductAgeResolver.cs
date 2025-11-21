using AutoMapper;
using ProductManagement.Features.Product;

namespace ProductManagement.Mapper;

public class ProductAgeResolver : IValueResolver<Product, ProductProfileDto, string>
{
    public string Resolve(Product source, ProductProfileDto dest, string destMember, ResolutionContext context)
    {
        var days = (DateTime.UtcNow - source.ReleaseDate).Days;

        if (days < 30) return "New Release";
        if (days < 365) return $"{days / 30} months old";
        if (days < 1825) return $"{days / 365} years old";
        if (days == 1825) return "Classic";
        return $"{days / 365} years old";
    }
}
