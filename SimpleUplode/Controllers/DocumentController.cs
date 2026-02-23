using ExcelDBAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UglyToad.PdfPig;
using System.Security.Claims;

namespace ExcelDBAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly MongoService _mongo;
        private readonly QdrantService _qdrant;

        public DocumentController(MongoService mongo, QdrantService qdrant)
        {
            _mongo = mongo;
            _qdrant = qdrant;
        }

        [Authorize(Roles = "User")]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadPdf(IFormFile file)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var filePath = Path.GetTempFileName();
            using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            string text = "";
            using (var pdf = PdfDocument.Open(filePath))
            {
                foreach (var page in pdf.GetPages())
                {
                    text += page.Text;
                }
            }

            var documentId = Guid.NewGuid().ToString();

            var doc = new DocumentData
            {
                DocumentId = documentId,
                FileName = file.FileName,
                Text = text,
                UserId = userId
            };

            _mongo.Insert(doc);
            await _qdrant.Insert(text, documentId);

            return Ok(new
            {
                message = "PDF uploaded & indexed successfully",
                documentId
            });
        }

      
        [Authorize(Roles = "User")]
        [HttpGet("my")]
        public IActionResult GetMyDocuments()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            return Ok(_mongo.GetByUserId(userId));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public IActionResult GetAllDocuments()
        {
            return Ok(_mongo.GetAll());
        }

        [Authorize]
        [HttpDelete("delete/{documentId}")]
        public IActionResult Delete(string documentId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            if (role == "Admin")
            {
                var result = _mongo.DeleteByDocumentId(documentId);
                if (!result) return NotFound();
                return Ok("Deleted by Admin");
            }

            var deleted = _mongo.DeleteUserDocument(documentId, userId);
            if (!deleted)
                return Forbid("You can delete only your own documents");

            return Ok("Deleted successfully");
        }

        [Authorize]
        [HttpGet("view/{documentId}")]
        public IActionResult ViewDocument(string documentId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            DocumentData doc;

            
            if (role == "Admin")
            {
                doc = _mongo.GetByDocumentId(documentId);
            }
            else
            {
               
                doc = _mongo.GetUserDocument(documentId, userId);
            }

            if (doc == null)
                return NotFound("Document not found");

            return Ok(new
            {
                documentId = doc.DocumentId,
                fileName = doc.FileName,
                text = doc.Text
            });
        }
    }
}
