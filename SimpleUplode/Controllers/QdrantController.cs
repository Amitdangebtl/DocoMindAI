using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExcelDBAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/qdrant")]
    public class QdrantController : ControllerBase
    {
        private readonly MongoService _mongo;
        private readonly QdrantService _qdrant;

        public QdrantController(MongoService mongo, QdrantService qdrant)
        {
            _mongo = mongo;
            _qdrant = qdrant;
        }

        // 🔹 Create / Ensure collection
        [HttpPost("create-collection")]
        public async Task<IActionResult> CreateCollection()
        {
            await _qdrant.EnsureCollection(); 
            return Ok("Collection created or already exists");
        }

        // 🔹 Index all Mongo data
        [HttpPost("index-all")]
        public async Task<IActionResult> IndexAll()
        {
            var docs = _mongo.GetAll();

            foreach (var doc in docs)
            {
                await _qdrant.Insert(doc.Text ,doc.DocumentId);
            }

            return Ok("All Mongo data indexed into Qdrant");
        }

        // 🔹 Search / Ask AI
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string query, string documentId)
        {
            var result = await _qdrant.AskAI(query,documentId);
            return Ok(result);
        }
    }
}
