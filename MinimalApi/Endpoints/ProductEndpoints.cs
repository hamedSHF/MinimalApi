using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using MinimalApi.DTO;
using MinimalApi.Infrastructure;
using MinimalApi.Infrastructure.Models;
using MinimalApi.Mappers;
using MinimalApi.Validators;
using System.Net;

namespace MinimalApi.Endpoints
{
    public static class ProductEndpoints
    {
        const string cacheKey = "product_list";
        public static RouteGroupBuilder MapProductEndpoints(this RouteGroupBuilder builder)
        {
            builder.MapPost("/add", AddProduct)
                .WithName("Add new prodouct")
                .WithDescription("Add new product")
                .WithSummary("Add new product")
                .Produces(StatusCodes.Status201Created, typeof(Results))
                .ProducesValidationProblem();
            builder.MapGet("", GetProducts)
                .WithName("Get all products")
                .WithDescription("Get all products")
                .WithSummary("Get all products")
                .Produces<List<IEnumerable<Product>>>();
            builder.MapGet("/{id:int}", GetProduct)
                .WithName("Get a product with id")
                .WithDescription("Get a single product with id")
                .WithSummary("Get a product using id")
                .Produces<Ok<Product>>()
                .ProducesValidationProblem();
            builder.MapGet("/cached", GetProductsWithCache)
                .WithName("Get cached products")
                .WithDescription("Get cached products")
                .WithSummary("Get cached products")
                .Produces<IEnumerable<Product>>();
            builder.MapPut("", UpdateProduct)
                .WithName("Fully update")
                .WithDescription("Fully update product")
                .WithSummary("Full update of a product")
                .Produces(StatusCodes.Status200OK, typeof(Results))
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesValidationProblem();
            builder.MapDelete("/delete/{id:int}", DeleteProduct)
                .WithName("Delete a product")
                .WithDescription("Delete a product using id")
                .WithSummary("Delete a product")
                .Produces(StatusCodes.Status200OK, typeof(Results))
                .ProducesProblem(StatusCodes.Status400BadRequest);

            return builder;
        }
        public static async Task<Results<Created ,ValidationProblem>> AddProduct(
            AddProductDto dto,
            ShopDbContext dbContext,
            IValidator<AddProductDto> validator)
        {
            var result = await validator.ValidateAsync(dto);
            if(!result.IsValid)
            {
                return TypedResults.ValidationProblem(result.ToDictionary());
            }
            var product = await dbContext.Products.AddAsync(dto.ToProduct());
            await dbContext.SaveChangesAsync();
            return TypedResults.Created($"product/{product.Entity.ProductID}");
        }
        public static async Task<IEnumerable<Product>> GetProducts(
            ShopDbContext dbContext)
        {
            return await dbContext.Products.ToListAsync();
        }
        public static async Task<IEnumerable<Product>> GetProductsWithCache(
            IMemoryCache cache,
            ShopDbContext dbContext)
        {
            if(!cache.TryGetValue(cacheKey, out IEnumerable<Product> products))
            {
                products = await dbContext.Products.ToListAsync();
                cache.Set(cacheKey, products, TimeSpan.FromMinutes(5));
            }
            return products;
        }
        public static async Task<Results<Ok<Product> ,BadRequest<string>>> GetProduct(
            [FromRoute] int id,
            ShopDbContext dbContext)
        {
            var product = await dbContext.Products.FindAsync(id);
            return product != null ? TypedResults.Ok(product) 
                : TypedResults.BadRequest($"Product with id {id} is not founded");
        }
        public static async Task<Results<Ok, ValidationProblem, BadRequest<string>>> UpdateProduct(
            UpdateProductDto product,
            ShopDbContext dbContext,
            IValidator<UpdateProductDto> validator)
        {
            var result = await validator.ValidateAsync(product);
            if(!result.IsValid)
            {
                return TypedResults.ValidationProblem(result.ToDictionary());
            }
            var item = await dbContext.Products.FirstOrDefaultAsync(x => x.ProductID == product.ProductID);
            if(item is null)
            {
                return TypedResults.BadRequest($"Product with id {product.ProductID} not founded");
            }
            item.Name = product.Name;
            item.Description = product.Description;
            item.Price = product.Price;
            await dbContext.SaveChangesAsync();
            return TypedResults.Ok();
        }
        public static async Task<Results<Ok, BadRequest<string>>> DeleteProduct(
            [FromRoute] int id,
            ShopDbContext dbContext)
        {
            var item = await dbContext.Products.FirstOrDefaultAsync(x => x.ProductID == id);
            if(item is null)
            {
                return TypedResults.BadRequest($"Product {id} not found.");
            }
            dbContext.Products.Remove(item);
            await dbContext.SaveChangesAsync();
            return TypedResults.Ok();
        }
    }
}
