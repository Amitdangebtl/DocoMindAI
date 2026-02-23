using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace ExcelDBAPI.Models
{
    public class DocumentData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonIgnore]
        public string Id { get; set; }
        public string DocumentId { get; set; }
        public string FileName { get; set; }
        public string Text { get; set; }
        public DateTime UploadedOn { get; set; }
        public string UserId { get; set; }
    }
}