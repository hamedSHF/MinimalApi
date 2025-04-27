
using Microsoft.EntityFrameworkCore;
using MinimalApi.Infrastructure;
using FluentValidation;
using MinimalApi.Validators;
using MinimalApi.Endpoints;

namespace MinimalApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<ShopDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("MinimalApiDb")));

            builder.Services.AddValidatorsFromAssemblyContaining<AddProductValidator>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapGroup("/product")
                .MapProductEndpoints()
                .WithTags("Product endpoints")
                .WithOpenApi();
            app.UseHttpsRedirection();

            app.Run();
        }
    }
}
