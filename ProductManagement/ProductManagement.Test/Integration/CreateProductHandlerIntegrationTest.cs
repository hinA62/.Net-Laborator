using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using ProductManagement.Exceptions;
using ProductManagement.Features.Product;
using ProductManagement.Features.Product.Handlers;
using ProductManagement.Logging;
using ProductManagement.Mapper;
using ProductManagement.Persistence;
using ProductManagement.Validators;
using Xunit;

namespace ProductManegement.Test.Integration
{
    public class CreateProductHandlerIntegrationTests : IDisposable
    {
        private readonly ProductsManagementContext _context;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly Mock<ILogger<CreateProductHandler>> _loggerMock;
        private readonly Mock<ILogger<CreateProductProfileValidator>> _validatorLoggerMock;
        private readonly CreateProductHandler _handler;

        public CreateProductHandlerIntegrationTests()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<ProductsManagementContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ProductsManagementContext(options);

            // Setup AutoMapper with both profiles
            var mapperConfig = new global::AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductMappingProfile>();
                cfg.AddProfile<AdvancedProductMappingProfile>();
            }, new LoggerFactory());
            _mapper = mapperConfig.CreateMapper();
            
            _cache = new MemoryCache(new MemoryCacheOptions());

            _loggerMock = new Mock<ILogger<CreateProductHandler>>();
            _validatorLoggerMock = new Mock<ILogger<CreateProductProfileValidator>>();

            _handler = new CreateProductHandler(_context, _loggerMock.Object, _validatorLoggerMock.Object, _mapper, _cache);
        }

        public void Dispose()
        {
            _context?.Dispose();
            (_cache as IDisposable)?.Dispose();
        }

        // -------------------------
        // Test 1: Valid Electronics Product
        // -------------------------
        [Fact]
        public async Task Handle_ValidElectronicsProductRequest_CreatesProductWithCorrectMappings()
        {
            // Arrange
            var request = new CreateProductProfileRequest(
                Name: "Super Tech Gadget",
                Brand: "Tech Brand",
                SKU: "ELEC-12345",
                Category: ProductCategory.Electronics,
                Price: 150m,
                ReleaseDate: DateTime.UtcNow.AddMonths(-2),
                ImageUrl: "https://example.com/image.png",
                IsAvailable: true,
                StockQuantity: 10
            );

            // Act
            var result = await _handler.Handle(request);

            // Assert: verify Created result
            Assert.IsAssignableFrom<Microsoft.AspNetCore.Http.IResult>(result);

            // Verify product persisted and mappings
            var product = _context.Products.FirstOrDefault(p => p.SKU == request.SKU);
            Assert.NotNull(product);

            var productDto = _mapper.Map<ProductProfileDto>(product);

            Assert.Equal("Electronics & Technology", productDto.CategoryDisplay);
            Assert.Equal("TB", productDto.BrandInitials); // Tech Brand -> TB
            Assert.Contains("months", productDto.ProductAge);
            Assert.StartsWith("$", productDto.PriceFormatted);
            Assert.Equal("In Stock", productDto.AvailabilityStatus);

            // Verify ProductCreationStarted log called once
            _loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Information),
                    It.Is<EventId>(e => e.Id == ProductLogEvents.ProductCreationStarted),
                    It.IsAny<object>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<object, Exception?, string>>()),
                Times.AtLeastOnce);
        }

        // -------------------------
        // Test 2: Duplicate SKU throws ValidationException
        // -------------------------
        [Fact]
        public async Task Handle_DuplicateSKU_ThrowsValidationExceptionWithLogging()
        {
            // Arrange: Existing product
            var existingProduct = new Product(
                Guid.NewGuid(),
                "Existing Gadget",
                "BrandX",
                "DUPL-001",
                ProductCategory.Electronics,
                120m,
                DateTime.UtcNow.AddMonths(-1),
                "https://example.com/img.png",
                true,
                5,
                null
            );
            _context.Products.Add(existingProduct);
            await _context.SaveChangesAsync();

            var request = new CreateProductProfileRequest(
                Name: "New Gadget",
                Brand: "BrandX",
                SKU: "DUPL-001", // duplicate SKU
                Category: ProductCategory.Electronics,
                Price: 130m,
                ReleaseDate: DateTime.UtcNow,
                ImageUrl: "https://example.com/img2.png",
                IsAvailable: true,
                StockQuantity: 5
            );

            // Act
            var ex = await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(request));
            
            // Assert
            Assert.Contains("already exists", ex.Errors.FirstOrDefault() ?? string.Empty, StringComparison.OrdinalIgnoreCase);

            // Verify ProductValidationFailed log called once (from validator path)
            _loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Warning),
                    It.Is<EventId>(e => e.Id == ProductLogEvents.ProductValidationFailed || e.Id == ProductLogEvents.SKUValidationPerformed),
                    It.IsAny<object>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<object, Exception?, string>>()),
                Times.AtLeastOnce);
        }

        // -------------------------
        // Test 3: Home Product Conditional Mapping
        // -------------------------
        [Fact]
        public async Task Handle_HomeProductRequest_AppliesDiscountAndConditionalMapping()
        {
            // Arrange
            var request = new CreateProductProfileRequest(
                Name: "Home Cleaning Kit",
                Brand: "HomeBrand",
                SKU: "HOME-001",
                Category: ProductCategory.Home,
                Price: 100m,
                ReleaseDate: DateTime.UtcNow.AddMonths(-6),
                ImageUrl: "https://example.com/home.png",
                IsAvailable: true,
                StockQuantity: 15
            );

            // Act
            var result = await _handler.Handle(request);

            // Assert
            var product = _context.Products.FirstOrDefault(p => p.SKU == request.SKU);
            Assert.NotNull(product);

            var productDto = _mapper.Map<ProductProfileDto>(product);

            Assert.Equal("Home & Garden", productDto.CategoryDisplay);
            Assert.Equal(request.Price * 0.9m, productDto.Price);
            Assert.Null(productDto.ImageUrl);
        }
    }
}
