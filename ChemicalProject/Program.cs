using ChemicalProject.Controllers;
using ChemicalProject.Data;
using ChemicalProject.Helper;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
   .AddNegotiate();
builder.Services.AddScoped<UserAreaService>();

// layanan DbContext    
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// HANGFIRE
builder.Services.AddTransient<HangfireAuthorizationFilter>();
builder.Services.AddHangfire(config =>
    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
          .UseSimpleAssemblyNameTypeSerializer()
          .UseRecommendedSerializerSettings()
          .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
          {
              CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
              SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
              QueuePollInterval = TimeSpan.FromMinutes(1),
              UseRecommendedIsolationLevel = true,
              DisableGlobalLocks = true,
              PrepareSchemaIfNecessary = true,
              SchemaName = "CC_Schema"
          }));
builder.Services.AddHangfireServer();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserArea", policy => policy.RequireRole("UserArea"));
    options.AddPolicy("UserSupervisor", policy => policy.RequireRole("UserSupervisor"));
    options.AddPolicy("UserManager", policy => policy.RequireRole("UserManager"));
    options.AddPolicy("UserAdmin", policy => policy.RequireRole("UserAdmin"));
    options.AddPolicy("AllowedUsers", policy =>
        policy.RequireRole("UserAdmin", "UserArea", "UserManager", "UserSupervisor"));
});

builder.Services.AddRazorPages();

var app = builder.Build();
    
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.Use(async (context, next) =>
{
    if (context.User.Identity.IsAuthenticated)
    {
        var identity = new ClaimsIdentity();
        var fullUsername = context.User.Identity.Name;
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var isUserArea = await dbContext.UserAreas.AnyAsync(a => a.UserName == fullUsername);
        var isUserSuperVisor = await dbContext.UserSuperVisors.AnyAsync(a => a.UserName == fullUsername);
        var isUserManager = await dbContext.UserManagers.AnyAsync(a => a.UserName == fullUsername);
        var isUserAdmin = await dbContext.UserAdmins.AnyAsync(a => a.UserName == fullUsername);

        if (isUserArea)
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, "UserArea"));
        }

        if (isUserSuperVisor)
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, "UserSuperVisor"));
        }

        if (isUserManager)
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, "UserManager"));
        }

        if (isUserAdmin)
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, "UserAdmin"));
        }

        context.User.AddIdentity(identity);
    }

    await next();
});

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter() },
    StatsPollingInterval = 600000,
    IgnoreAntiforgeryToken = true
});

RecurringJob.AddOrUpdate<ExpireCheckController>(x => x.CheckExpiration(), Cron.Daily(23, 0));
RecurringJob.AddOrUpdate<StockCheckController>(x => x.CheckStock(), Cron.Daily(23, 0));

app.Run();

