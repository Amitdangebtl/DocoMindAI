using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using SimpleUplode.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SimpleUplode.Services
{
    public class AuthService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IConfiguration _config;

        public AuthService(IConfiguration config)
        {
            _config = config;

            var client = new MongoClient(
                config["MongoDbSettings:ConnectionString"]
            );

            var database = client.GetDatabase(
                config["MongoDbSettings:DatabaseName"]
            );

            _users = database.GetCollection<User>(
                config["MongoDbSettings:UserCollection"]
            );
        }

        // REGISTER
        public string Register(RegisterRequest model)
        {
            var existingUser = _users.Find(x => x.Email == model.Email).FirstOrDefault();
            if (existingUser != null)
                throw new Exception("Email already registered");

            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),

                
                Role = "User"
            };

            _users.InsertOne(user);
            return "User Registered Successfully";
        }

        // LOGIN
        public string Login(LoginRequest model)
        {
            var user = _users.Find(x => x.Email == model.Email).FirstOrDefault();

            if (user == null)
                throw new Exception("Invalid Email");

            if (!BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                
                throw new Exception("Invalid Password");

            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.Id), 
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"])
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(
                    Convert.ToDouble(_config["Jwt:ExpiryMinutes"])
                ),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public List<User> GetAllUsers()
        {
            return _users.Find(_ => true).ToList();
        }

        public void DeleteUser(string id)
        {
            _users.DeleteOne(x => x.Id == id && x.Role != "Admin");
        }

        public User GetById(string id)
        {
            return _users.Find(x => x.Id == id).FirstOrDefault();
        }
        public void Update(User user)
        {
            _users.ReplaceOne(x => x.Id == user.Id, user);
        }
    }
}
