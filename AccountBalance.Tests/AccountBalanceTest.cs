using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Xunit;

namespace AccountBalance.Tests
{
    public class AccountBalanceTest
    {
        static HttpClient client = new HttpClient();
        static string accountId;
        public AccountBalanceTest()
        {
            client.BaseAddress = new Uri("http://localhost:34723");
            client.MaxResponseContentBufferSize = 5000;

            var values = new Dictionary<string, string>();
            values.Add("HolderName", "Saif");
            var jsonString = JsonConvert.SerializeObject(values);

            HttpResponseMessage response = client.PostAsync(
                "api/Account/new", new StringContent(jsonString, Encoding.UTF8, "application/json")).Result;

            response.EnsureSuccessStatusCode();
            dynamic jsonResponse = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
            accountId = jsonResponse.value;
        }

        [Fact]
        public void ShouldCreateAccount()
        {
            var expectedStartingWith = "account-";
            Assert.StartsWith(expectedStartingWith, accountId);
        }

        [Fact]
        public void ShouldReturnAccountById()
        {
            var expectedResult = new
            {
                holderName = "Saif",
                balance = (float)0,
                overDraftLimit = (float)0,
                dailyWireTransferLimit = (float)0,
                accountState = "actif",
                id = new { value = accountId }
            };
            var jsonExpectedResult = JsonConvert.SerializeObject(expectedResult);
            HttpResponseMessage response = client.GetAsync(
                "api/Account/" + accountId).Result;
            var jsonResponse = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(jsonExpectedResult, jsonResponse);
        }

        [Fact]
        public void ShouldDepositCash()
        {
            var expectedResult = new
            {
                holderName = "Saif",
                balance = (float)500,
                overDraftLimit = (float)0,
                dailyWireTransferLimit = (float)0,
                accountState = "actif",
                id = new { value = accountId }
            };
            var jsonExpectedResult = JsonConvert.SerializeObject(expectedResult);

            var values = new Dictionary<string, dynamic>();
            values.Add("Amount", 500);
            values.Add("AccountId", accountId);
            var jsonString = JsonConvert.SerializeObject(values);
            HttpResponseMessage response = client.PutAsync(
                "api/Account/Cash/deposit", new StringContent(jsonString, Encoding.UTF8, "application/json")).Result;
            HttpResponseMessage responseAfter = client.GetAsync(
                "api/Account/" + accountId).Result;
            var jsonResponse = responseAfter.Content.ReadAsStringAsync().Result;


            Assert.Equal(jsonExpectedResult, jsonResponse);
        }
    }
}
