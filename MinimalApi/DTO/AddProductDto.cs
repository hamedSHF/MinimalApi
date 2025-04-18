using MinimalApi.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace MinimalApi.DTO
{
    public class AddProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}
