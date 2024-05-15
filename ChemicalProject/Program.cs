using ChemicalProject.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using ChemicalProject.Helper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
   .AddNegotiate();

// layanan DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("AreaPolicy", policy => policy.RequireAssertion(context =>
    {
        var role = context.User.FindFirst(ClaimTypes.Role)?.Value;
        var area = context.User.FindFirst("Area")?.Value;
        return (role == "Manager" || role == "Supervisor" || role == "User") && !string.IsNullOrEmpty(area);
    }));

    options.AddPolicy("ApprovalPolicy", policy => policy.RequireAssertion(context =>
    {
        var role = context.User.FindFirst(ClaimTypes.Role)?.Value;
        var area = context.User.FindFirst("Area")?.Value;
        return role == "Manager" && !string.IsNullOrEmpty(area);
    }));
});

builder.Services.AddRazorPages();

// Register custom user service
builder.Services.AddScoped<IUserService, UserService>();

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
app.UseAuthorization();

app.UseMiddleware<RoleAreaMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
