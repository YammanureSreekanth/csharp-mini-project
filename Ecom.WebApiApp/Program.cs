using Microsoft.EntityFrameworkCore;
using Ecom.WebApiApp.Data;
using Ecom.WebApiApp.Repos;
using Microsoft.OpenApi;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        OpenApiTag? productTag = document.Tags?.FirstOrDefault(t => t.Name == "Product");
        if (productTag is not null)
        {
            productTag.Description = "This is Product API Group. Here you can perform GET, POST, PUT, DELETE";
        }
        return Task.CompletedTask;
    });
});

string? DbCon = builder.Configuration.GetConnectionString("DbCon");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
   options.UseSqlServer(DbCon);
});

builder.Services.AddScoped<IProductRepository, ProductRepository>();

WebApplication? app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "/openapi/v1.json";
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
