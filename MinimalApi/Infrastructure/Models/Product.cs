namespace MinimalApi.Infrastructure.Models
{
    public class Product
    {
        public int ProductID { get; private set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set;}
    }
}
