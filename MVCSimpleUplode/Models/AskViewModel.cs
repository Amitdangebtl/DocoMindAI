using Microsoft.AspNetCore.Mvc.Rendering;

namespace MVCSimpleUplode.Models
{
    public class AskViewModel
    {
        public string Question { get; set; }
        public string DocumentId { get; set; }
        public string Answer { get; set; }

        public List<SelectListItem> Documents { get; set; } = new();
    }
}