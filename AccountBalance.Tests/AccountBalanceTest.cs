using AccountBalance.Api.Controllers;
using AccountBalance.Application.Interfaces;
using System.Net.Http;
using Xunit;

namespace AccountBalance.Tests
{
    public class AccountBalanceTest
    {
        static HttpClient client = new HttpClient();
        static string accountId;
        [Fact]
        public async void Test1()
        {

            HttpResponseMessage response = await client.GetAsync(
                "api/products" );
            response.EnsureSuccessStatusCode();
        }
    }
}
