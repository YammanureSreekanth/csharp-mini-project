using Ecom.MvcWebApp.Data;
using Microsoft.EntityFrameworkCore;
using Middlewares;
using Services;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IMessageService, MessageService>();

builder.Services.AddSingleton<SingletonService>();
builder.Services.AddScoped<ScopedService>();
builder.Services.AddTransient<TransientService>();

string? connectionString = builder.Configuration.GetConnectionString("DbCon");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/blocked")
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        await context.Response.WriteAsync("Access to this page is blocked.");
    }
    await next();
});

app.Map("/Health", helloBatch =>
{
    helloBatch.Run(async context =>
    {
        Console.WriteLine("Testing Map Route with RUN termnial");
        context.Response.StatusCode = StatusCodes.Status200OK;
        await context.Response.WriteAsync("This is healthy");
    });
});

app.UseWhen(context => context.Request.Path == "/Home", homeBranch =>
{
    homeBranch.Use(async (context, next) =>
    {
        Console.WriteLine("Use only for Home Router");
        await next();
    });
});

app.MapWhen(context => context.Request.Query["noFurther"] == true, NoFurtherBranch =>
{
    NoFurtherBranch.Run(async context =>
    {
        Console.WriteLine("Entering into NoFurther pipeline");
        await context.Response.WriteAsync("This is the end");
    });
});

app.MapWhen(context => context.Request.Headers["X-Diagnostic"] == true, NoFurtherBranch =>
{
    NoFurtherBranch.Run(async context =>
    {
        Console.WriteLine("Entering into NoFurther pipeline");
        await context.Response.WriteAsync("This is the end");
    });
});

app.Map("/No-DI", NoDIBranch =>
{
    NoDIBranch.Run(async context =>
    {
        IMessageService messageService = new MessageService();
        await context.Response.WriteAsync(messageService.GetMessage());
    });
});

app.Map("/With-DI", WithDIBranch =>
{
    WithDIBranch.Run(async context =>
    {
        IMessageService messageService = context.RequestServices.GetRequiredService<IMessageService>();
        await context.Response.WriteAsync(messageService.GetMessage());
    });
});


app.Map("/Lifetimes", lifetimesBranch =>
{
    lifetimesBranch.Run(async context =>
    {
        SingletonService singletonService1 = context.RequestServices.GetRequiredService<SingletonService>();
        SingletonService singletonService2 = context.RequestServices.GetRequiredService<SingletonService>();

        ScopedService scopedService1 = context.RequestServices.GetRequiredService<ScopedService>();
        ScopedService scopedService2 = context.RequestServices.GetRequiredService<ScopedService>();

        TransientService transientService1 = context.RequestServices.GetRequiredService<TransientService>();
        TransientService transientService2 = context.RequestServices.GetRequiredService<TransientService>();

        context.Response.ContentType = "text/pain";

        await context.Response.WriteAsync(
            $"""
            Service Lifetimes

            Singleton: Same object everytime
            First: {singletonService1.Id}
            Second: {singletonService2.Id}

            Scoped: one object per request life
            First: {scopedService1.Id}
            Second: {scopedService2.Id}

            Transient: New object on every time requesting
            First: {transientService1.Id}
            Second: {transientService2.Id}

            """
        );

    });
});


app.UseMiddleware<RequestLoggingMiddleware>();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
