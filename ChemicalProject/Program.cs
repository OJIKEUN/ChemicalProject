using ChemicalProject.Data;
using ChemicalProject.Helper;
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

app.Run();
