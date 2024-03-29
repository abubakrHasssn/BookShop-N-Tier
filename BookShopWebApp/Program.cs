
using BookShop.DataAccess.Data;
using BookShop.DataAccess.Repository;
using BookShop.DataAccess.Repository.IRepository;
using BookShop.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Stripe;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")));
//assign values to StripeClass 
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
builder.Services.AddIdentity<IdentityUser,IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
builder.Services.AddScoped<IUnitOFWork, UnitOFWork>();
builder.Services.AddSingleton<IEmailSender, EmailSender>();
builder.Services.AddRazorPages();
builder.Services.ConfigureApplicationCookie(option =>
{
    option.LoginPath = $"/Identity/Account/login";
    option.LogoutPath = $"/Identity/Account/logout";
    option.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});
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
//configure stripe ApiKey
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
app.UseAuthentication();

app.UseAuthorization();
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");
app.Run();
