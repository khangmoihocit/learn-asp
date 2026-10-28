using Microsoft.AspNetCore.Authentication.OAuth;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages().AddRazorPagesOptions(options =>
{
    options.RootDirectory = "/Pages"; //thay doi noi luu razor
    options.Conventions.AddPageRoute("/FirstPage", "/trang-day-tien.html"); //thay doi ten url, hoặc cấuhinh theo cách @page ""
});

builder.Services.Configure<RouteOptions>(routeOptions =>
{
   routeOptions.LowercaseUrls = true; 
});
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseRouting();
app.MapRazorPages();
app.MapGet("/", () => "Hello World!");

app.Run();
