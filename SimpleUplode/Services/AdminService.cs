using SimpleUplode.Models;
using ExcelDBAPI.Models;

namespace SimpleUplode.Services
{
    public class AdminService
    {
        private readonly AuthService _authService;
        private readonly MongoService _mongo;

        public AdminService(AuthService authService, MongoService mongo)
        {
            _authService = authService;
            _mongo = mongo;
        }
        public IEnumerable<User> GetAllUsers()
        {
            return _authService.GetAllUsers();
        }

        public User GetUserById(string id)
        {
            return _authService.GetById(id);
        }

        public void DeleteUser(string id)
        {
            _authService.DeleteUser(id);
        }

  
        public IEnumerable<DocumentData> GetAllDocuments()
        {
            return _mongo.GetAll();
        }
    }
}