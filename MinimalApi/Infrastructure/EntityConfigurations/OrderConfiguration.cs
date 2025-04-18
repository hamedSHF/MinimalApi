using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinimalApi.Infrastructure.Models;

namespace MinimalApi.Infrastructure.EntityConfigurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(x => new {x.OrderID,x.OrderDetailID});
            builder.Property(x => x.OrderDetailID)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Qutantity)
                .IsRequired();
        }
    }
}
