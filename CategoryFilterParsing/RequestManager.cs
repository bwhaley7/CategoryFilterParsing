using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace CategoryFilterParsing
{
    internal class RequestManager
    {
        private static HttpClient httpClient;
        private string baseUrl = "https://www.tsconline.com/api/v2/products?includeSuggestions=true&stockedItemsOnly=true&search=";
        private string urlSuffix = "&expand=attributes%2Cfacets%2CvariantTraits%2Cbadges&applyPersonalization=true&includeAttributes=includeOnProduct&onException=function%20()%20%7B%20%5Bnative%20code%5D%20%7D";

        public RequestManager()
        {
            httpClient = new HttpClient();
        }

        public async Task GetResponse(string search)
        {

            var query = Uri.EscapeDataString(search);
            var url = baseUrl + query + urlSuffix;

            HttpResponseMessage response = await httpClient.GetAsync(url);
            if(response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                ApiResponse apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseBody);

                if (apiResponse?.CategoryFacets != null)
                {
                    //Console.WriteLine($"Response for {search}:");
                    foreach (var facet in apiResponse.CategoryFacets)
                    {
                        //Console.WriteLine($"- {facet.ShortDescription}: Number of Products: {facet.Count}");
                    }
                }
                else
                    Console.WriteLine($"No category facets found for {search}");
            }
            else
                Console.WriteLine($"Failed to get response for {search}");
        }

        public async Task GetResponse(Category cat)
        {
            string search = "";
            if (string.IsNullOrEmpty(cat.GetName())){
                search = cat.GetTopLevel();
            }
            else{
                search = cat.GetName();
            }
            var query = Uri.EscapeDataString(search);
            var url = baseUrl + query + urlSuffix;

            HttpResponseMessage response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                ApiResponse apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseBody);

                if (apiResponse?.CategoryFacets != null)
                {
                    foreach (var facet in apiResponse.CategoryFacets)
                    {
                        //Console.WriteLine($"- {facet.ShortDescription}: Number of Products: {facet.Count}");
                        cat.categoryFacets.Add(facet);
                    }
                }
                else
                    Console.WriteLine($"No category facets found for {search}");
            }
            else
                Console.WriteLine($"Failed to get response for {search}");
        }

        public async Task GetCategoryData(List<Category> categories)
        {
            foreach(var cat in categories)
            {
                await GetResponse(cat);
            }
        }
    }
}
