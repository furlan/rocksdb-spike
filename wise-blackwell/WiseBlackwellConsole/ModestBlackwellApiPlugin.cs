using System.ComponentModel;
using Microsoft.SemanticKernel;

public class ModestBlackwellApiPlugin
{
    protected string AssetsApiUrl = "http://localhost:8080/api/assets/";
    protected string graphqlUrl = "http://localhost:8080/graphql";

    [KernelFunction("get_assets")]
    [Description("Gets a list of all assets and their attributes")]
    public async Task<string> GetAllAssetsAsync()
    {
        try
        {
            using HttpClient client = new HttpClient();
            // Skip SSL certificate validation for localhost (development only)
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            using var httpClient = new HttpClient(handler);

            HttpResponseMessage response = await httpClient.GetAsync(AssetsApiUrl);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Assets retrieved successfully:");
            Console.WriteLine(responseBody);

            return responseBody;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error retrieving assets: {ex.Message}");
            return null;
        }
    }

    [KernelFunction("get_asset")]
    [Description("Gets the attributes of a specific asset")]
    public async Task<string> GetAssetAsync(string assetId)
    {
        try
        {
            using HttpClient client = new HttpClient();
            // Skip SSL certificate validation for localhost (development only)
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            using var httpClient = new HttpClient(handler);

            var url = AssetsApiUrl + assetId;
            Console.WriteLine(url);
            HttpResponseMessage response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Asset ID {assetId} retrieved successfully:");
            Console.WriteLine(responseBody);

            return responseBody;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error retrieving asset id {assetId}: {ex.Message}");
            return null;
        }
    }

    [KernelFunction("get_streams")]
    [Description("Gets the streams data of a specific asset")]
    public async Task<string> GetAssetWithOperationalDataAsync(string assetId)
    {
        try
        {
            // Skip SSL certificate validation for localhost (development only)
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            using var httpClient = new HttpClient(handler);

            // Create the GraphQL query
            string graphqlQuery = $"{{ \"query\": \"{{asset(id: \\\"{assetId}\\\")  {{ type {{ name streams {{ id assetId values {{ key value }}}}}}}}}}\" }}";

            // Create the content to be sent
            var content = new StringContent(graphqlQuery, System.Text.Encoding.UTF8, "application/json");

            // Send the POST request
            HttpResponseMessage response = await httpClient.PostAsync(graphqlUrl, content);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Asset streams for {assetId} retrieved successfully:");
            Console.WriteLine(responseBody);

            return responseBody;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error retrieving streams for asset id {assetId}: {ex.Message}");
            return null;
        }
    }
}