using System.Net.Http.Headers;

namespace Journal_Service;

public class ClarivateApiClient
{
    private static readonly HttpClient client = new HttpClient();

    public ClarivateApiClient()
    {
        // Base address of the Clarivate API
        client.BaseAddress = new Uri("https://api.clarivate.com");
        // Set the default headers
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Add("apiKey", "your-api-key-here"); // Replace with your actual API key
    }

    public async Task<string> GetRequestAsync(string endpoint)
    {
        HttpResponseMessage response = await client.GetAsync(endpoint);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();
        }
        else
        {
            throw new Exception($"Request failed with status code {response.StatusCode}");
        }
    }

    public async Task<string> PostRequestAsync(string endpoint, HttpContent content)
    {
        HttpResponseMessage response = await client.PostAsync(endpoint, content);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();
        }
        else
        {
            throw new Exception($"Request failed with status code {response.StatusCode}");
        }
    }
}