using SimpleUplode.Models;

namespace SimpleUplode.Services
{
    public class UserService
    {
        private readonly AuthService _authService;

        public UserService(AuthService authService)
        {
            _authService = authService;
        }

        public User GetById(string id)
        {
            return _authService.GetById(id);
        }

        public void UpdateUser(string id, UpdateUserRequest model)
        {
            var user = _authService.GetById(id);
            if (user == null)
                throw new Exception("User not found");

            user.Name = model.Name;
            user.Email = model.Email;

            _authService.Update(user);  
        }
    }
}