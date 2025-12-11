using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using InvestorCenter.Areas.Identity.Data;
using InvestorCenter.Data;
using InvestorCenter.Hubs;
using InvestorCenter.Services;

var builder = WebApplication.CreateBuilder(args);
//var connectionString = builder.Configuration.GetConnectionString("InvestorCenterContextConnection") ?? throw new InvalidOperationException("Connection string 'InvestorCenterContextConnection' not found.");;
var connectionString = "Data Source=InvestorCenter.db";
builder.Services.AddDbContext<InvestorCenterContext>(options => options.UseSqlite(connectionString));

builder.Services.AddDefaultIdentity<InvestorCenterUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<InvestorCenterContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHostedService<StockPriceService>();
builder.Services.AddSignalR();
builder.Services.AddSingleton<StockUpdateSettings>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapRazorPages();

app.MapHub<StockHub>("/stockHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<InvestorCenterUser>>();

    // Create the "Admin" role if it doesn't exist
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    // Find your user by email and assign them the "Admin" role
    var adminUser = await userManager.FindByEmailAsync("adibogdan2004@gmail.com");
    if (adminUser != null && !await userManager.IsInRoleAsync(adminUser, "Admin"))
    {
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}

app.Run();
