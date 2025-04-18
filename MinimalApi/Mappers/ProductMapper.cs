using MinimalApi.DTO;
using MinimalApi.Infrastructure.Models;

namespace MinimalApi.Mappers
{
    public static class ProductMapper
    {
        public static Product ToProduct(this AddProductDto dto)
            => new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price
            };
        public static Product ToProduct(this UpdateProductDto dto)
            => new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price
            };
    }
}
