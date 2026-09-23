using Microsoft.EntityFrameworkCore;
using Ecom.WebApiApp.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        var productTag = document.Tags?.FirstOrDefault(t => t.Name == "Product");
        if (productTag is not null)
        {
            productTag.Description = "This is Product API Group. Here you can perform GET, POST, PUT, DELETE";
        }
        return Task.CompletedTask;
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
   options.UseInMemoryDatabase("Catalog"); 
});

var app = builder.Build();

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
