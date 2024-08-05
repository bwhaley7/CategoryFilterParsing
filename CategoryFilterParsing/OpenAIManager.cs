using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace CategoryFilterParsing
{
    internal class OpenAIManager
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        private readonly string _prompt;

        public OpenAIManager()
        {
            _httpClient = new HttpClient();
            _apiKey = GetAPIKey();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
            _prompt = "You will be given an output that shows a search term and its returned category facets. You will  be ranking the category facets from 0% - 100% based on how relevant they are to the search term. The relevancy score will be used to determine if the facet should be removed. The website is from a company that wholesales plumbing and HVAC products. Please split the facets into Highly Relevant, Moderately Relevant, Low Relevance, and Remove. Include the search term in the output so it is easy to understand what term the facets belong to.";
        }

        private string GetAPIKey()
        {
            var builder = new ConfigurationBuilder()
                .AddUserSecrets<Program>();
            var config = builder.Build();
            return config["OpenAI:ApiKey"];
        }

        public async Task<string> GetChatCompletionAsync(string prompt)
        {
            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "system", content = _prompt },
                    new { role = "user", content = prompt }
                },
                temperature = 0.3

            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
            response.EnsureSuccessStatusCode();

            string respondeBody = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(respondeBody);

            return jsonResponse.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
        }
    }
}
