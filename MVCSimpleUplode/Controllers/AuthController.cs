using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MVCSimpleUplode.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

public class AuthController : Controller
{
    private readonly HttpClient _http;
    public AuthController(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("API");
    }
    //public AuthController(IHttpClientFactory factory)
    //{
    //    _http = factory.CreateClient();
    //    _http.BaseAddress = new Uri("https://localhost:7167/api/");
    //}

    // ================= LOGIN =================
    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginRequest());
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginRequest model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var response = await _http.PostAsync(
            "auth/login",
            new StringContent(
                JsonSerializer.Serialize(model),
                Encoding.UTF8,
                "application/json"));

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "Invalid Email or Password";
            return View(model);
        }

        var result = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(result);

        var token = doc.RootElement.GetProperty("token").GetString();

        // ===== SAVE TOKEN =====
        HttpContext.Session.SetString("Token", token);

        // ===== DECODE JWT =====
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        // ===== EXTRACT ROLE (FIXED) =====
        var role =
            jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value
            ?? jwt.Claims.FirstOrDefault(c => c.Type == "role")?.Value
            ?? jwt.Claims.FirstOrDefault(c => c.Type == "roles")?.Value;

        // ===== CLAIMS =====
        var claims = new List<Claim>
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? ""
            ),
            new Claim(
                ClaimTypes.Email,
                jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? ""
            )
        };

        // ===== ADD ROLE CLAIM (MOST IMPORTANT) =====
        if (!string.IsNullOrEmpty(role))
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // ===== COOKIE SIGN-IN =====
        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal);

        // ===== ROLE BASED REDIRECT =====
        if (role == "Admin")
            return RedirectToAction("Dashboard", "Admin");

        return RedirectToAction("Dashboard", "User");
    }

    // ================= REGISTER =================
    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterRequest());
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterRequest model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var response = await _http.PostAsync(
            "auth/register",
            new StringContent(
                JsonSerializer.Serialize(model),
                Encoding.UTF8,
                "application/json"));

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "Registration failed";
            return View(model);
        }

        return RedirectToAction("Login");
    }

    // ================= LOGOUT =================
    public async Task<IActionResult> Logout()
    {
        HttpContext.Session.Clear();

        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction("Login");
    }
}