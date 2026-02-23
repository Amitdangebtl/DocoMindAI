using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleUplode.Models;
[Authorize]
[ApiController]
[Route("api/ai")]
public class AiController : ControllerBase
{
    private readonly QdrantService _qdrant;

    public AiController(QdrantService qdrant)
    {
        _qdrant = qdrant;
    }
    [Authorize]
    [HttpPost("ask")]
    public async Task<IActionResult> Ask([FromBody] AskRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Question))
            return BadRequest("Question is required");

        if (string.IsNullOrWhiteSpace(request.DocumentId))
            return BadRequest("DocumentId is required");

        var answer = await _qdrant.AskAI(
            request.Question,
            request.DocumentId
        );

        return Ok(new
        {
            question = request.Question,
            answer
        });
    }

}
