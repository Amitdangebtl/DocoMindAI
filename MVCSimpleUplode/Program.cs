using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// ---------------- MVC ----------------
builder.Services.AddControllersWithViews();

// ---------------- HTTP CONTEXT (LAYOUT / ROLES) ----------------
builder.Services.AddHttpContextAccessor();

// ---------------- SESSION ----------------
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ---------------- HTTP CLIENT (API CALLS) ----------------
builder.Services.AddHttpClient("API", (sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();

    // Docker ke liye yahi URL use hoga
    var apiBaseUrl = config["ApiBaseUrl"] ?? "http://webapi/api/";

    client.BaseAddress = new Uri(apiBaseUrl);
});

// ---------------- AUTHENTICATION (COOKIE BASED MVC) ----------------
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    });

// ---------------- AUTHORIZATION ----------------
builder.Services.AddAuthorization();

var app = builder.Build();

// ---------------- PIPELINE ----------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// ⚠️ Docker me HTTPS force mat karo
// app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// ---------------- ROUTING ----------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();