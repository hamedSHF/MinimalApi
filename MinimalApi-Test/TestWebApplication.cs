using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MinimalApi.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using MinimalApi;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MinimalApi_Test
{
    public class TestWebApplication : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptionsBuilder<ShopDbContext>));
                services.RemoveAll(typeof(ShopDbContext));

                services.AddDbContext<ShopDbContext>(options =>
                {
                    options.UseSqlServer(
                    "Server=HamSh;Database=MinimalApi_Test;" +
                    "Integrated Security=True;Trust Server Certificate=True;Trusted_Connection=True;");
               });
                var context = services.BuildServiceProvider().CreateScope().ServiceProvider.GetRequiredService<ShopDbContext>();
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            });
        } 
    }
}
