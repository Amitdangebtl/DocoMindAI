using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVCSimpleUplode.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

[Authorize(Roles = "User")]
public class UserController : Controller
{
    private readonly HttpClient _http;

    public UserController(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("API");
    }
    //public UserController(IHttpClientFactory factory)
    //{
    //    _http = factory.CreateClient();
    //    _http.BaseAddress = new Uri("https://localhost:7167/api/");
    //}
    private void AttachToken()
    {
        var token = HttpContext.Session.GetString("Token");
        if (!string.IsNullOrEmpty(token))
        {
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }

    // -------- DASHBOARD --------
    public IActionResult Dashboard()
    {
        return View();
    }

    // -------- PROFILE --------
    public async Task<IActionResult> Profile()
    {
        AttachToken();

        var response = await _http.GetAsync("user/me");
        if (!response.IsSuccessStatusCode)
            return RedirectToAction("Login", "Auth");

        var json = await response.Content.ReadAsStringAsync();
        var model = JsonSerializer.Deserialize<UserProfileViewModel>(
            json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return View(model);
    }

    // -------- EDIT PROFILE --------
    [HttpGet]
    public async Task<IActionResult> EditProfile()
    {
        AttachToken();

        var response = await _http.GetAsync("user/me");
        var json = await response.Content.ReadAsStringAsync();

        var model = JsonSerializer.Deserialize<UpdateUserRequest>(
            json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> EditProfile(UpdateUserRequest model)
    {
        if (!ModelState.IsValid)
            return View(model);

        AttachToken();

        var response = await _http.PutAsync(
            "user/me",
            new StringContent(
                JsonSerializer.Serialize(model),
                Encoding.UTF8,
                "application/json"));

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "Profile update failed";
            return View(model);
        }

        return RedirectToAction(nameof(Profile));
    }
}