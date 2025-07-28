using ModestBlackwell.Services;
using ModestBlackwell.Services.Interfaces;

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
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Configure data path
builder.Configuration["DataPath"] = Path.Combine(builder.Environment.ContentRootPath, "data");

// Register services
builder.Services.AddScoped<IAssetService, AssetService>();
builder.Services.AddScoped<IStreamService, StreamService>();

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

app.Run();
