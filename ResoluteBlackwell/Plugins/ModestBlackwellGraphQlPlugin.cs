using System.ComponentModel;
using Microsoft.SemanticKernel;

public class ModestBlackwellGraphQlPlugin 
{
    protected string graphqlUrl = "http://localhost:8080/graphql";

    [KernelFunction("execute_graphql_query")]
    [Description("Get asset, streams, and operational data using the GraphQL query")]
    public async Task<string> GetAssetStreamWithOperationalDataUsingGraphQlAsync(string query)
    {
        try
        {
            // Skip SSL certificate validation for localhost (development only)
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            using var httpClient = new HttpClient(handler);
            var graphqlQuery = $"{{ \"query\": \"{query.Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t")}\" }}";

            // Create the content to be sent
            var content = new StringContent(query, System.Text.Encoding.UTF8, "application/json");

            Console.WriteLine(query);

            // Send the POST request
            HttpResponseMessage response = await httpClient.PostAsync(graphqlUrl, content);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"GraphQL quere response: ");
            Console.WriteLine(responseBody);

            return responseBody;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error executing GraphQL query: {ex.Message}");
            return null;
        }
    }
}