using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Session2._1
{
    [TestClass]
    public class UnitTest1
    {
        private static HttpClient httpClient;
        private static readonly string BaseUrl = "https://petstore.swagger.io/v2/";
        private static readonly string endpoint = "pet";
        private static string GetURL(string endpoint) => $"{BaseUrl}{endpoint}";
        private static Uri GetURI(string endpoint) => new Uri(GetURL(endpoint));

        private readonly List<PetModel> cleanUpList = new List<PetModel>();


        [TestInitialize]
        public void TestInitialize()
        {
            httpClient = new HttpClient();
        }

        [TestCleanup]
        public async Task TestCleanup()
        {
            foreach ( var data in cleanUpList )
            {
                var httpResponse = await httpClient.DeleteAsync(GetURI($"{endpoint}/{data.Id}"));
            }
        }

        [TestMethod]
        public async Task TestMethod1()
        {
            PetModel pet = new PetModel()
            {
                Category = new Category { Name = "Labrador" },
                Name = "Choco",
                PhotoUrls = new string[] { "photo " },
                Tags = new Category[] { new Category { Name = "Tags" } },
                Status = "available"
            };

            var request = JsonConvert.SerializeObject(pet);
            var postRequest = new StringContent(request, Encoding.UTF8, "application/json");

            var httpResponse = await httpClient.PostAsync(GetURI(endpoint), postRequest);

            var obj = JsonConvert.DeserializeObject<PetModel>(httpResponse.Content.ReadAsStringAsync().Result);

            pet = new PetModel()
            {
                Id = obj.Id,
                Category = new Category { Name = "Labrador" },
                Name = "Lucky",
                PhotoUrls = new string[] { "photo " },
                Tags = new Category[] { new Category { Name = "Tags" } },
                Status = "available"
            };

            request = JsonConvert.SerializeObject(pet);
            postRequest = new StringContent(request, Encoding.UTF8, "application/json");

            httpResponse = await httpClient.PutAsync(GetURI(endpoint), postRequest);

            var updatedPet = JsonConvert.DeserializeObject<PetModel>(httpResponse.Content.ReadAsStringAsync().Result);

            cleanUpList.Add(updatedPet);

            Assert.AreEqual(httpResponse.StatusCode, HttpStatusCode.OK, "Status code is not equal to 200");
            Assert.AreEqual(pet.Name, updatedPet.Name, "Pet name does not match");

        }
    }
}