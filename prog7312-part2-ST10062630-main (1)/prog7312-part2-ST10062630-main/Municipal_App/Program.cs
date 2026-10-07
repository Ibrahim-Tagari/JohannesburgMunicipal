using Microsoft.EntityFrameworkCore;
using Municipal_App.Data;
using Municipal_App.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("MunicipalServicesDb"));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IIssueService, IssueService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IPushSubscriptionService, PushSubscriptionService>();
builder.Services.AddScoped<ILocalEventService, LocalEventService>();
builder.Services.AddScoped<IEventRecommendationService, EventRecommendationService>();
builder.Services.AddScoped<IEventDataStructureService, EventDataStructureService>();
builder.Services.AddScoped<IDatabaseManagementService, DatabaseManagementService>();
builder.Services.AddScoped<IServiceRequestStatusService, ServiceRequestStatusService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(24);
    });

builder.Services.AddSession();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

    if (!context.Users.Any())
    {
        // Create admin user
        await userService.CreateUserAsync(new Municipal_App.Models.User
        {
            Username = "admin",
            Email = "admin@test.com",
            FullName = "Admin User",
            Role = "Admin",
            PasswordHash = userService.HashPassword("admin123")
        });

        // Create regular user
        await userService.CreateUserAsync(new Municipal_App.Models.User
        {
            Username = "user",
            Email = "user@test.com",
            FullName = "Test User",
            Role = "User",
            PasswordHash = userService.HashPassword("user123")
        });
    }
}

app.Run();
