using Microsoft.AspNetCore.Http;
using MinimalApi.DTO;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace MinimalApi_Test.IntegerationTests
{
    public class ProductApiTests : IClassFixture<TestWebApplication>
    {
        private readonly TestWebApplication factory;
        private readonly HttpClient client;
        private readonly ITestOutputHelper output;
        public ProductApiTests(TestWebApplication factory, ITestOutputHelper output)
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
            output.WriteLine((await response.Content.ReadAsStringAsync()));
            Assert.NotNull(response);
        }
        public static IEnumerable<object[]> CreateData =>
            new List<object[]>
            {
                new object[] {new AddProductDto { Name = "", Description = "Red and blue", Price = 12} },
                new object[] {new AddProductDto { Name = "Laptop", Description = "Manufactured by HP", Price = 0} }
            };
        [Theory]
        [MemberData(nameof(CreateData))]
        public async Task CreateProduct_With_ValidationProblems(AddProductDto product)
        {
            var response = await client.PostAsJsonAsync("/product/add", product);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var problemResult = await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
            foreach (var item in problemResult?.Errors)
            {
                output.WriteLine(item.Key + ":");
                foreach(var error in item.Value)
                {
                    output.WriteLine(error);
                }
            }
            Assert.NotNull(problemResult?.Errors);
        }
        [Fact]
        public async Task CreateProduct_Without_ValidationProblems()
        {
            var product = new AddProductDto
            {
                Name = "Nike Shoe",
                Description = "Red and blue colors",
                Price = 15
            };
            var response = await client.PostAsJsonAsync("/product/add", product);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }
    }
}
