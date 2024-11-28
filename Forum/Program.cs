using Microsoft.EntityFrameworkCore;
using Forum.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Add logging services
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders(); // Clears default providers
    logging.AddConsole(); // Add console logging
    logging.AddDebug(); // Add debug logging (optional, useful in Visual Studio)
    logging.SetMinimumLevel(LogLevel.Information); // Set minimum log level to Information (can be adjusted)
});

// Add session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true; // Secure the session cookie
    options.Cookie.IsEssential = true;
});

// Add MVC services (for controllers and views)
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Default HSTS value is 30 days
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession(); // Enable session middleware

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Question}/{action=Index}/{id?}");

app.Run();
