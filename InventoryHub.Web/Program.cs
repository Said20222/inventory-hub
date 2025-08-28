using InventoryHub.Web.Data;
using InventoryHub.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsql => npgsql.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(10), errorCodesToAdd: null)
        ).EnableDetailedErrors()
        .EnableSensitiveDataLogging(builder.Environment.IsDevelopment()));

builder.Services
    .AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = builder.Environment.IsProduction();
        options.User.RequireUniqueEmail = true;
    })
    .AddRoles<IdentityRole>()                 
    .AddEntityFrameworkStores<AppDbContext>() 
    .AddDefaultTokenProviders()              
    .AddDefaultUI();  

builder.Services.AddScoped<IAccessService, AccessService>();                 
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// TODO: Remove this in production
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        const string adminRole = "Admin";
        if (!await roleMgr.RoleExistsAsync(adminRole))
            await roleMgr.CreateAsync(new IdentityRole(adminRole));

        // (Optional) Seed admin user
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Admin/role seeding failed; continuing without seeding");
    }
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
