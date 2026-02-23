using ExcelDBAPI.Models;
using MongoDB.Driver;
using SimpleUplode.Models;

public class MongoService
{
    private readonly IMongoDatabase _db;

    public MongoService(IConfiguration config)
    {
        var client = new MongoClient(
            config["MongoDbSettings:ConnectionString"]
        );

        _db = client.GetDatabase(
            config["MongoDbSettings:DatabaseName"]
        );
    }

    public IMongoCollection<User> users =>
        _db.GetCollection<User>("Users");

    
    public IMongoCollection<DocumentData> documents =>
        _db.GetCollection<DocumentData>("Document");


    public void Insert(DocumentData data)
    {
        documents.InsertOne(data);
    }

    public List<DocumentData> GetAll()
    {
        return documents.Find(_ => true).ToList();
    }

    public List<DocumentData> GetByUserId(string userId)
    {
        return documents.Find(x => x.UserId == userId).ToList();
    }

    public bool DeleteByDocumentId(string documentId)
    {
        var result = documents.DeleteOne(x => x.DocumentId == documentId);
        return result.DeletedCount > 0;
    }

    public bool DeleteUserDocument(string documentId, string userId)
    {
        var filter = Builders<DocumentData>.Filter.And(
            Builders<DocumentData>.Filter.Eq(x => x.DocumentId, documentId),
            Builders<DocumentData>.Filter.Eq(x => x.UserId, userId)
        );

        var result = documents.DeleteOne(filter);
        return result.DeletedCount > 0;
    }

   

    public User GetUserById(string id)
    {
        return users.Find(x => x.Id == id).FirstOrDefault();
    }

    public List<User> GetAllUsers()
    {
        return users.Find(_ => true).ToList();
    }

    public void InsertUser(User user)
    {
        users.InsertOne(user);
    }

    public void UpdateUser(User user)
    {
        users.ReplaceOne(x => x.Id == user.Id, user);
    }

    public void DeleteUser(string id)
    {
        users.DeleteOne(x => x.Id == id);
    }

    public DocumentData GetByDocumentId(string documentId)
    {
        return documents.Find(d => d.DocumentId == documentId).FirstOrDefault();
    }

    public DocumentData GetUserDocument(string documentId, string userId)
    {
        return documents.Find(d =>
            d.DocumentId == documentId && d.UserId == userId).FirstOrDefault();
    }
}