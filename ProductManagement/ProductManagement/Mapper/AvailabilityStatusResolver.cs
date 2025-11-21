using AutoMapper;
using ProductManagement.Features.Product;

namespace ProductManagement.Mapper;

public class AvailabilityStatusResolver : IValueResolver<Product, ProductProfileDto, string>
{
    public string Resolve(Product source, ProductProfileDto dest, string destMember, ResolutionContext context)
    {
        if (!source.IsAvailable) return "Out of Stock";

        return source.StockQuantity switch
        {
            0 => "Unavailable",
            1 => "Last Item",
            <= 5 => "Limited Stock",
            _ => "In Stock"
        };
    }
}
