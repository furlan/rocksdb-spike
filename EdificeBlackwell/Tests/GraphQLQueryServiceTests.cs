using EdificeBlackwell.Models;
using EdificeBlackwell.Services;

namespace EdificeBlackwell.Tests;

/// <summary>
/// Simple test class to verify GraphQL query generation functionality
/// </summary>
public class GraphQLQueryServiceTests
{
    private readonly GraphQLQueryService _service;

    public GraphQLQueryServiceTests()
    {
        _service = new GraphQLQueryService();
    }

    /// <summary>
    /// Test query generation with both location and operational data type
    /// </summary>
    public void TestQueryWithLocationAndType()
    {
        var intent = new QueryIntent
        {
            Location = "living room",
            OperationalDataType = "alarm",
            IsRelevant = true
        };

        var query = _service.GenerateQuery(intent);
        
        if (!query.Contains("asset(location: \"Living Room\")"))
            throw new Exception("Query should contain location filter");
        
        if (!query.Contains("type(name: \"alarm\")"))
            throw new Exception("Query should contain type filter");

        Console.WriteLine("✅ Test passed: Query with location and type");
    }

    /// <summary>
    /// Test query generation with only operational data type
    /// </summary>
    public void TestQueryWithTypeOnly()
    {
        var intent = new QueryIntent
        {
            Location = null,
            OperationalDataType = "notification",
            IsRelevant = true
        };

        var query = _service.GenerateQuery(intent);
        
        if (query.Contains("asset(location:"))
            throw new Exception("Query should not contain location filter");
        
        if (!query.Contains("type(name: \"notification\")"))
            throw new Exception("Query should contain type filter");

        Console.WriteLine("✅ Test passed: Query with type only");
    }

    /// <summary>
    /// Test query generation with only location
    /// </summary>
    public void TestQueryWithLocationOnly()
    {
        var intent = new QueryIntent
        {
            Location = "kitchen",
            OperationalDataType = null,
            IsRelevant = true
        };

        var query = _service.GenerateQuery(intent);
        
        if (!query.Contains("asset(location: \"Kitchen\")"))
            throw new Exception("Query should contain location filter");
        
        if (query.Contains("type(name:"))
            throw new Exception("Query should not contain type filter");

        Console.WriteLine("✅ Test passed: Query with location only");
    }

    /// <summary>
    /// Test irrelevant query handling
    /// </summary>
    public void TestIrrelevantQuery()
    {
        var intent = new QueryIntent
        {
            Location = null,
            OperationalDataType = null,
            IsRelevant = false
        };

        var query = _service.GenerateQuery(intent);
        
        if (!query.Contains("# Query not relevant"))
            throw new Exception("Query should indicate it's not relevant");

        Console.WriteLine("✅ Test passed: Irrelevant query handling");
    }

    /// <summary>
    /// Test operational data service functionality
    /// </summary>
    public void TestOperationalDataService()
    {
        var operationalService = new OperationalDataService();
        
        // Test loading operational types
        var types = operationalService.GetOperationalTypes();
        if (types.Count != 3)
            throw new Exception($"Expected 3 operational types, got {types.Count}");
        
        // Test synonym lookup
        var found = operationalService.FindOperationalType("temperature");
        if (found?.Name != "utilization")
            throw new Exception("'temperature' should map to 'utilization'");
        
        found = operationalService.FindOperationalType("warning");
        if (found?.Name != "alarm")
            throw new Exception("'warning' should map to 'alarm'");
        
        found = operationalService.FindOperationalType("message");
        if (found?.Name != "notification")
            throw new Exception("'message' should map to 'notification'");

        Console.WriteLine("✅ Test passed: Operational data service");
    }

    /// <summary>
    /// Run all tests
    /// </summary>
    public static void RunAllTests()
    {
        Console.WriteLine("Running EdificeBlackwell tests...");
        Console.WriteLine();

        var tests = new GraphQLQueryServiceTests();
        
        try
        {
            tests.TestQueryWithLocationAndType();
            tests.TestQueryWithTypeOnly();
            tests.TestQueryWithLocationOnly();
            tests.TestIrrelevantQuery();
            tests.TestOperationalDataService();
            
            Console.WriteLine();
            Console.WriteLine("🎉 All tests passed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Test failed: {ex.Message}");
            Environment.Exit(1);
        }
    }
}
