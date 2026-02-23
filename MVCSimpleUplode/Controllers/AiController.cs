using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVCSimpleUplode.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

[Authorize]
public class AiController : Controller
{
    private readonly HttpClient _http;

    public AiController(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("API");
    }

    //public AiController(IHttpClientFactory factory)
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

    // ---------- GET : Ask Page ----------
    [HttpGet]
    public async Task<IActionResult> Ask()
    {
        AttachToken();

        var model = new AskViewModel();
        HttpResponseMessage response;

        // 🔐 ROLE BASED DOCUMENT LOAD
        if (User.IsInRole("Admin"))
        {
            // Admin → all documents
            response = await _http.GetAsync("document/all");
        }
        else
        {
            // User → only own documents
            response = await _http.GetAsync("document/my");
        }

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var docs = JsonSerializer.Deserialize<List<DocumentViewModel>>(
                json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            foreach (var d in docs)
            {
                model.Documents.Add(new SelectListItem
                {
                    // Admin ke liye filename + user
                    Text = User.IsInRole("Admin")
                        ? $"{d.FileName} (User: {d.UserId})"
                        : d.FileName,

                    Value = d.DocumentId
                });
            }
        }

        return View(model);
    }

    // ---------- POST : Ask AI ----------
    [HttpPost]
    public async Task<IActionResult> Ask(AskViewModel model)
    {
        AttachToken();

        if (string.IsNullOrWhiteSpace(model.DocumentId) ||
            string.IsNullOrWhiteSpace(model.Question))
        {
            ViewBag.Error = "Please select document and enter question";

            HttpResponseMessage resp;

            if (User.IsInRole("Admin"))
                resp = await _http.GetAsync("document/all");
            else
                resp = await _http.GetAsync("document/my");

            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                var docs = JsonSerializer.Deserialize<List<DocumentViewModel>>(
                    json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                model.Documents = docs.Select(d => new SelectListItem
                {
                    Text = User.IsInRole("Admin")
                        ? $"{d.FileName} (User: {d.UserId})"
                        : d.FileName,
                    Value = d.DocumentId
                }).ToList();
            }

            return View(model);
        }

        // -------- ASK AI API CALL --------
        var payload = new
        {
            question = model.Question,
            documentId = model.DocumentId
        };

        var response = await _http.PostAsync(
            "ai/ask",
            new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"));

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "AI could not generate answer";
            return View(model);
        }

        var result = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(result);

        model.Answer = doc.RootElement.GetProperty("answer").GetString();

        return View(model);
    }
}