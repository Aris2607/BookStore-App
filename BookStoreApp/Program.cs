using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BookStoreApp.Data;
using BookStoreApp.Data.Interfaces;
using BookStoreApp.Services;
using BookStoreApp.Middlewares;
using BookStoreApp.Models;
using Nest;

var builder = WebApplication.CreateBuilder(args);

//var settings = new ConnectionSettings(new Uri("https://localhost:9200"))
//    .DefaultIndex("books");

//var client = new ElasticClient(settings);

// Not Secure For Production
//var settings = new ConnectionSettings(new Uri("https://localhost:9200"))
//    .ServerCertificateValidationCallback((sender, certificate, chain, sslPolicyErrors) => true);

//var client = new ElasticClient(settings);

//builder.Services.AddSingleton<IElasticClient>(client);


// Configure database connection
builder.Services.AddDbContext<BookStoreAppContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BookStoreAppContext") ??
        throw new InvalidOperationException("Connection string 'BookStoreAppContext' not found.")));

// Configure session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<BookStoreAppContext>()
.AddDefaultTokenProviders();

// Add HttpContextAccessor for middleware/session
builder.Services.AddHttpContextAccessor();

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IErrorService, ErrorService>();

// Register Elasticsearch service
//builder.Services.AddScoped<ElasticsearchService>();

var app = builder.Build();


// Ensure Elasticsearch indexing happens during app startup
//using (var scope = app.Services.CreateScope())
//{
//    var elasticsearchService = scope.ServiceProvider.GetRequiredService<ElasticsearchService>();
//    await elasticsearchService.IndexBooksAsync();
//}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthentication(); // Ensure Identity is configured
app.UseAuthorization();
app.UseMiddleware<CartMiddleware>(); // Custom middleware
app.UseStatusCodePagesWithReExecute("/Errors/{0}");

// Konfigurasi routing untuk error pages
app.MapControllerRoute(
    name: "Errors",
    pattern: "Errors/{statusCode}",
    defaults: new { controller = "Errors", action = "HandleError" }
);


// Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
