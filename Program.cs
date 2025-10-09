using Microsoft.OpenApi.Models;
using putovanjeApp1.Services;
using Neo4jClient;

var builder = WebApplication.CreateBuilder(args);



var client = new BoltGraphClient(
    new Uri("bolt://localhost:7687"),
    "neo4j",
    "password"
);
client.DefaultDatabase = "putovanjeapp";

await client.ConnectAsync();
//client.DefaultDatabase = "putovanjeapp";
builder.Services.AddSingleton<IGraphClient>(client);



// Dodaj servis
//builder.Services.AddScoped<Neo4jService>();
builder.Services.AddScoped<UserService>();
// Dodaj kontrolere
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SmartTrip API",
        Version = "v1"
    });
});

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartTrip API v1");
    c.RoutePrefix = string.Empty; 
});

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
