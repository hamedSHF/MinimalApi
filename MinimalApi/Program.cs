
using Microsoft.EntityFrameworkCore;
using MinimalApi.Infrastructure;
using FluentValidation;
using MinimalApi.Validators;
using MinimalApi.Endpoints;
using AspNetCoreRateLimit;

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

            builder.Services.AddMemoryCache();

            builder.Services.Configure<IpRateLimitOptions>(options =>
            {
                options.GeneralRules = new List<RateLimitRule>
                {
                    new RateLimitRule
                    {
                        Endpoint = "*",
                        Limit = 5,
                        Period = "1m"
                    }
                };
            });

            builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            builder.Services.AddInMemoryRateLimiting();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins", policy =>
                {
                    policy.WithOrigins("https://www.example.com")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
                });
                options.AddPolicy("AllowAllOrigins", policy =>
                {
                    policy.AllowAnyOrigin();
                    policy.AllowAnyMethod();
                });
            });

            builder.Services.AddValidatorsFromAssemblyContaining<AddProductValidator>();

            var app = builder.Build();

            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseIpRateLimiting();
            app.UseCors();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapGroup("/product")
                .MapProductEndpoints()
                .WithTags("Product endpoints")
                .WithOpenApi()
                .RequireCors("AllowAllOrigins");
            app.UseHttpsRedirection();

            app.Run();
        }
    }
}
