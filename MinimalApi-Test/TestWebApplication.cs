using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MinimalApi.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using MinimalApi;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace MinimalApi_Test
{
    public class TestWebApplication : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var dbcontext = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(ShopDbContext));
                if (dbcontext != null)
                {
                    services.Remove(dbcontext);
                    var options = services.Where(r => (r.ServiceType == typeof(DbContextOptions))
                      || (r.ServiceType.IsGenericType && r.ServiceType.GetGenericTypeDefinition() == typeof(DbContextOptions<>))).ToArray();
                    foreach (var option in options)
                    {
                        services.Remove(option);
                    }
                }
                services.AddDbContext<ShopDbContext>(options =>
                {
                    options.UseSqlServer("Server=HamSh;Database=MinimalApi_Test;Integrated Security=True;Trust Server Certificate=True;Trusted_Connection=True;");
               });
                var context = services.BuildServiceProvider().CreateScope().ServiceProvider.GetRequiredService<ShopDbContext>();
                context.Database.EnsureCreated();
            });
        } 
    }
}
