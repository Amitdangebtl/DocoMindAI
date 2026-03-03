🚀 DocuMindAI

Full-Stack AI-powered Document Intelligence System built using:

🖥 ASP.NET Core MVC (UI Layer)

🔌 ASP.NET Core Web API (Backend)

🍃 MongoDB (NoSQL Database)

🧠 OpenAI + Qdrant (Vector Search)

🔐 JWT + HMAC Security

🏗️ Solution Architecture

This solution contains 2 Projects:

1️⃣ MVCSimpleUplode (Frontend - MVC)

Responsible for:

User Interface (Razor Views)

Authentication UI (Login/Register)

Admin Dashboard

Document Upload UI

AI Question Interface

API Integration using HttpClient

Main Folders
Controllers/
Views/
Models/
wwwroot/

Important Controllers:

AdminController

AiController

AuthController

DocumentController

UserController

2️⃣ SimpleUplode (Backend - Web API)

Responsible for:

REST APIs

Business Logic

MongoDB Operations

AI Integration

Security Middleware

Subscription Validation

Main Folders
Controllers/
Models/
Services/
Filters/
Security/
Important Components

✔ QdrantController
✔ DocumentController
✔ AiController
✔ AuthController
✔ SubscriptionFilter
✔ HmacMiddleware
✔ OpenAIService
✔ QdrantService
✔ MongoServices

🧠 How System Works

User registers & logs in (JWT generated)

User uploads document

Document stored in MongoDB

Embeddings generated using OpenAI

Embeddings stored in Qdrant

User asks question

System searches vector DB

Context sent to OpenAI

AI response returned

🛠️ Technologies Used

ASP.NET Core MVC

ASP.NET Core Web API

C#

MongoDB

OpenAI API

Qdrant

JWT Authentication

HMAC Request Validation

Custom Filters

Docker

🔐 Security Features

JWT Authentication

HMAC Signature Validation

Subscription-Based Access Filter

Middleware-based Request Protection

⚙️ How to Run
1️⃣ Configure MongoDB

Update appsettings.json:

"MongoDbSettings": {
  "ConnectionString": "mongodb://localhost:27017",
  "DatabaseName": "DocuMindAI"
}
2️⃣ Run Backend (API)
dotnet run --project SimpleUplode
3️⃣ Run Frontend (MVC)
dotnet run --project MVCSimpleUplode
🐳 Run with Docker
docker-compose up --build
📌 Major API Endpoints
Upload Document

POST /api/document/upload

Ask AI

POST /api/ai/ask

{
  "question": "Explain .NET Developer role",
  "documentId": "document-id"
}
🎯 Key Highlights

✔ Full Stack Architecture
✔ Secure API Design
✔ AI-Powered Document Q&A
✔ Vector Search Integration
✔ Subscription Based Feature Control
✔ Clean Layered Structure

👨‍💻 Author

Amit Dange
.NET Developer | AI Integration | MongoDB
