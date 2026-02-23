using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVCSimpleUplode.Models;
using System.Net.Http.Headers;
using System.Text.Json;

[Authorize]
public class DocumentController : Controller
{
    private readonly HttpClient _http;

    public DocumentController(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("API");
    }
    //public DocumentController(IHttpClientFactory factory)
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

    // -------- UPLOAD (USER) --------
    [Authorize(Roles = "User")]
    [HttpGet]
    public IActionResult Upload()
    {
        return View();
    }

    [Authorize(Roles = "User")]
    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        AttachToken();

        if (file == null || file.Length == 0)
        {
            ViewBag.Error = "Please select a PDF file";
            return View();
        }

        var content = new MultipartFormDataContent();
        content.Add(new StreamContent(file.OpenReadStream()), "file", file.FileName);

        var response = await _http.PostAsync("document/upload", content);

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "Upload failed";
            return View();
        }

        return RedirectToAction(nameof(MyDocuments));
    }

    // -------- MY DOCUMENTS (USER) --------
    [Authorize(Roles = "User")]
    public async Task<IActionResult> MyDocuments()
    {
        AttachToken();

        var response = await _http.GetAsync("document/my");
        var json = await response.Content.ReadAsStringAsync();

        var model = JsonSerializer.Deserialize<List<DocumentViewModel>>(
            json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? new List<DocumentViewModel>();

        return View(model);
    }

    // -------- ALL DOCUMENTS (ADMIN) --------
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AllDocuments()
    {
        AttachToken();

        var response = await _http.GetAsync("document/all");
        var json = await response.Content.ReadAsStringAsync();

        var model = JsonSerializer.Deserialize<List<DocumentViewModel>>(
            json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? new List<DocumentViewModel>();

        return View(model);
    }

    // -------- DELETE (USER / ADMIN) --------
    [HttpPost]
    public async Task<IActionResult> Delete(string documentId)
    {
        AttachToken();

        await _http.DeleteAsync($"document/delete/{documentId}");

        return User.IsInRole("Admin")
            ? RedirectToAction(nameof(AllDocuments))
            : RedirectToAction(nameof(MyDocuments));
    }

    // -------- VIEW DOCUMENT (USER) --------
    [Authorize(Roles = "User")]
    [HttpGet]
    public async Task<IActionResult> ViewDocument(string documentId)
    {
        AttachToken();

        var response = await _http.GetAsync($"document/view/{documentId}");
        if (!response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(MyDocuments));
        }

        var json = await response.Content.ReadAsStringAsync();

        var model = JsonSerializer.Deserialize<DocumentViewModel>(
            json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return View(model);
    }
}