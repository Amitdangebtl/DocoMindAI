using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OfficeOpenXml;
using SimpleUplode.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =======================
// 🔧 Render PORT SUPPORT
// =======================
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://*:{port}");

// =======================
// 📊 EPPlus License
// =======================
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

// =======================
// 🎮 Controllers
// =======================
builder.Services.AddControllers();

// =======================
// 🔌 Services
// =======================
builder.Services.AddSingleton<MongoService>();
builder.Services.AddHttpClient<OpenAIService>();
builder.Services.AddSingleton<QdrantService>();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<UserService>();

// =======================
// 🔐 JWT Authentication
// =======================
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
        )
    };
});

// =======================
// 🔐 Authorization
// =======================
builder.Services.AddAuthorization();

// =======================
// 🌐 CORS (Allow All)
// =======================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// =======================
// 📄 Swagger + JWT
// =======================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "DocoMindAI API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT like: Bearer {token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// =======================
// 🚀 Middleware Pipeline
// =======================

// ✅ Swagger ENABLED for Render (NO IsDevelopment check)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "DocoMindAI API v1");
    c.RoutePrefix = "swagger"; // /swagger
});

app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection();

// 🔐 ORDER MATTERS
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();