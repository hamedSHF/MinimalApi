using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using MinimalApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace MinimalApi_Test.IntegerationTests
{
    public class ProductApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> factory;
        private readonly HttpClient client;
        private readonly ITestOutputHelper output;
        public ProductApiTests(WebApplicationFactory<Program> factory, ITestOutputHelper output)
        {
            this.factory = factory;
            this.client = factory.CreateClient();
            this.output = output;
        }
        [Theory]
        [InlineData("/product")]
        public async Task GetAllProducts_Test(string path)
        {
            var response = await client.GetAsync(path);
            output.WriteLine(await response.Content.ReadAsStringAsync());
            Assert.NotNull(response);
        }
    }
}
