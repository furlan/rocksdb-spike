using Microsoft.SemanticKernel;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using WBStrucOut.Models;
using System.Text.Json;

var modelId = "gtp-4-0125-Preview";
var endpoint = "https://ff-openai-gpt-4.openai.azure.com/";
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY environment variable is not set.");

// create a kernel with Azure OpenAI chat completion
var builder = Kernel.CreateBuilder().AddAzureOpenAIChatCompletion(modelId, endpoint, apiKey);
Kernel kernel = builder.Build();

// Step 1: Define the prompt
var prompt = @"
You are a helpful assistant that extracts structured queries from user input.
User: {{$input}}
Return the result in this JSON format:
{""operation"": ""query"", ""entity"": ""Employee"", ""filters"": {""joinDate"": {""gt"": ""2020-01-01""}}}
";

var function = kernel.CreateFunctionFromPrompt(prompt, functionName: "ExtractEmployeeQuery");

// Step 2: Get structured output from Semantic Kernel
Console.WriteLine("Enter your query:");
var userInput = Console.ReadLine();

var result = await kernel.InvokeAsync(function, new() { ["input"] = userInput });
var json = result.GetValue<string>();

Console.WriteLine("\nStructured Output:");
Console.WriteLine(json);

// Step 3: Deserialize to C# object
var query = JsonSerializer.Deserialize<EmployeeQuery>(json);

// Step 4: Build GraphQL query
string BuildGraphQLQuery(EmployeeQuery q)
{
    var entity = q.Entity.ToLower();
    var filter = q.Filters["joinDate"]["gt"];

    return $@"
query {{
  {entity}s(where: {{ joinDate_gt: ""{filter}"" }}) {{
    id
    name
    joinDate
  }}
}}";
}

var gqlQuery = BuildGraphQLQuery(query);

// Step 5: Execute GraphQL query
var gqlClient = new GraphQLHttpClient("https://your-graphql-endpoint.com/graphql", new NewtonsoftJsonSerializer()); // Replace with your endpoint

var gqlRequest = new GraphQLRequest { Query = gqlQuery };
/*
try
{
    var gqlResponse = await gqlClient.SendQueryAsync<dynamic>(gqlRequest);

    Console.WriteLine("\nGraphQL Response:");
    Console.WriteLine(JsonSerializer.Serialize(gqlResponse.Data, new JsonSerializerOptions { WriteIndented = true }));
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
*/