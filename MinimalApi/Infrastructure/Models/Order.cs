namespace MinimalApi.Infrastructure.Models
{
    public class Order
    {
        public Guid OrderID { get; set; }
        public int OrderDetailID { get; private set; }
        public int Qutantity { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
