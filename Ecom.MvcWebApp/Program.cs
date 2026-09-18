using Middlewares;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

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

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
