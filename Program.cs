using dotnet_project_ecommerce.Models;
using dotnet_project_ecommerce.sessionhelper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor(); 

// Add database context
builder.Services.AddDbContext<myContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("myconnection")));

// Add session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10); 
    options.Cookie.HttpOnly = true; 
    options.Cookie.IsEssential = true; 
});

// Configure file upload limits
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 50 * 1024 * 1024; 
});

// Configure Kestrel for large file uploads
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 50 * 1024 * 1024; 
});

var app = builder.Build();

// Configure SessionHelper
var httpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();
SessionHelper.Configure(httpContextAccessor);

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); 
    app.UseHsts(); 
}

app.UseHttpsRedirection(); 
app.UseStaticFiles(); 
app.UseRouting(); 
app.UseSession(); 
app.UseAuthorization(); 

// Configure default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Customer}/{action=Index}/{id?}"
);

app.Run(); 