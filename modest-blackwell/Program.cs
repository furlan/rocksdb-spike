using ModestBlackwell.Services;
using ModestBlackwell.Services.Interfaces;
using ModestBlackwell.GraphQL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "ModestBlackwell API", 
        Version = "v1",
        Description = "API for accessing automation equipment data in a house"
    });
    
    // Include XML comments for better Swagger documentation
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Configure data path
builder.Configuration["DataPath"] = System.IO.Path.Combine(builder.Environment.ContentRootPath, "data");

// Register services
builder.Services.AddScoped<IAssetService, AssetService>();
builder.Services.AddScoped<IStreamService, StreamService>();
builder.Services.AddScoped<IRocksDbService, RocksDbService>();
builder.Services.AddScoped<IGraphQLService, GraphQLService>();

// Add GraphQL
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddProjections()
    .AddFiltering()
    .AddSorting();

// Add logging
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ModestBlackwell API v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the root
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Map GraphQL endpoint
app.MapGraphQL("/graphql");

// Initialize RocksDB on startup
using (var scope = app.Services.CreateScope())
{
    var rocksDbService = scope.ServiceProvider.GetRequiredService<IRocksDbService>();
    await rocksDbService.InitializeDatabaseAsync();
    
    // Seed test data for demonstration
    // Temporarily disabled due to RocksDB native library issues
    // var dataSeedingService = new DataSeedingService(
    //     scope.ServiceProvider.GetRequiredService<ILogger<DataSeedingService>>(),
    //     scope.ServiceProvider.GetRequiredService<IConfiguration>()
    // );
    // dataSeedingService.SeedTestData();
}

app.Run();
