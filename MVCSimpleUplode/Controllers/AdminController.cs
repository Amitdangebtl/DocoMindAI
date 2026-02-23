using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVCSimpleUplode.Models;
using System.Net.Http.Headers;
using System.Text.Json;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly HttpClient _http;

    public AdminController(IHttpClientFactory factory)
    {

        _http = factory.CreateClient("API");
    }

    //public AdminController(IHttpClientFactory factory)
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

    // -------- Dashboard --------
    public IActionResult Dashboard()
    {
        return View();
    }

    // -------- Users --------
    public async Task<IActionResult> Users()
    {
        AttachToken();

        var response = await _http.GetAsync("admin/users");
        if (!response.IsSuccessStatusCode)
            return View(new List<AdminUserViewModel>());

        var json = await response.Content.ReadAsStringAsync();
        var model = JsonSerializer.Deserialize<List<AdminUserViewModel>>(
            json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return View(model);
    }

    // -------- Delete User --------
    [HttpPost]
    public async Task<IActionResult> DeleteUser(string id)
    {
        AttachToken();
        await _http.DeleteAsync($"admin/user/{id}");
        return RedirectToAction(nameof(Users));
    }

    // -------- Documents --------
    public async Task<IActionResult> Documents()
    {
        AttachToken();

        var response = await _http.GetAsync("admin/documents");
        if (!response.IsSuccessStatusCode)
            return View(new List<AdminDocumentViewModel>());

        var json = await response.Content.ReadAsStringAsync();
        var model = JsonSerializer.Deserialize<List<AdminDocumentViewModel>>(
            json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return View(model);
    }

    // -------- VIEW DOCUMENT (ADMIN) --------
    public async Task<IActionResult> ViewDocument(string documentId)
    {
        AttachToken();

        var response = await _http.GetAsync($"document/view/{documentId}");
        if (!response.IsSuccessStatusCode)
            return RedirectToAction(nameof(Documents));

        var json = await response.Content.ReadAsStringAsync();
        var model = JsonSerializer.Deserialize<DocumentViewModel>(
            json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return View(model);
    }
}