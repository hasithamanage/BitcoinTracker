using System.Net.Http;
using System.Reflection.PortableExecutable;

namespace BitcoinTracker.Services
{
    // This class handles all HTTP GET requests to the CoinGecko API and returning the raw JSON response as a string
    public class ApiClient
    {
        // A single reusable instance of HttpClient for the lifetime of the ApiClient.
        // HttpClient is thread-safe and should not be frequently instantiated.
        private readonly HttpClient _httpClient;

        // Sets up the base API URL and adds the CoinGecko API key to the request headers.
        public ApiClient()
        {
            
            // Initialize HttpClient with the base address for CoinGecko API
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.coingecko.com/api/v3/")
            };

            // Add the CoinGecko demo API key to each request header
            _httpClient.DefaultRequestHeaders.Add("x-cg-demo-api-key", "CG-2uzQyVNvqpNtTpJi15C8bgUd");
        }


        // Sends a GET request to the specified endpoint and returns the response as a string
        public async Task<string> GetAsync(string endpoint)
        {
            // Execute HTTP GET request to the given endpoint
            var response = await _httpClient.GetAsync(endpoint);

            // Check for success — CoinGecko API returns 200 OK when valid
            if (!response.IsSuccessStatusCode)
            {
                // If request fails, capture the detailed error message from response body
                var errorContent = await response.Content.ReadAsStringAsync();

                // Throw an exception with full error context (useful for debugging)
                throw new HttpRequestException(
                    $"API request failed: {(int)response.StatusCode} {response.ReasonPhrase}\n{errorContent}"
                );
            }

            // Return raw JSON data as string for further deserialization
            return await response.Content.ReadAsStringAsync();
        }
    }
}
