# 🚀 DocuMindAI  

AI-powered Document Intelligence System built using **ASP.NET Core MVC (UI)** + **ASP.NET Core Web API (Backend)** + **MongoDB** + **OpenAI + Qdrant**.

---

## 📌 Project Overview  

DocuMindAI is a full-stack AI-based document processing system that allows users to:

- 📄 Upload documents  
- 🗄️ Store document data in MongoDB  
- 🧠 Generate embeddings using OpenAI  
- 📊 Store embeddings in Qdrant (Vector Database)  
- ❓ Ask AI questions based on uploaded documents  
- 🔐 Secure APIs using JWT & HMAC  
- 💳 Restrict features using Subscription-based access  

---

## 🏗️ Solution Architecture  

This solution contains **2 Projects**:

### 1️⃣ MVCSimpleUplode (Frontend - MVC)

Responsible for:

- User Interface (Razor Views)  
- Authentication (Login / Register)  
- Admin Dashboard  
- Document Upload UI  
- AI Question Interface  
- API Integration using HttpClient  

**Main Structure:**

```
Controllers/
Views/
Models/
wwwroot/
```

---

### 2️⃣ SimpleUplode (Backend - Web API)

Responsible for:

- REST APIs  
- Business Logic  
- MongoDB Operations  
- OpenAI Integration  
- Qdrant Vector Storage  
- Security Middleware  
- Subscription Validation  

**Main Structure:**

```
Controllers/
Models/
Services/
Filters/
Security/
```

Important Components:

- AiController  
- DocumentController  
- AuthController  
- QdrantController  
- SubscriptionFilter  
- HmacMiddleware  
- OpenAIService  
- QdrantService  
- MongoServices  

---

## 🔐 Security Features  

- ✅ JWT Authentication  
- ✅ HMAC Signature Validation  
- ✅ Subscription-Based Access Control  
- ✅ Middleware-based Request Protection  

---

## 🛠️ Technologies Used  

- ASP.NET Core MVC  
- ASP.NET Core Web API  
- C#  
- MongoDB  
- OpenAI API  
- Qdrant  
- JWT Authentication  
- Docker  
- Swagger  

---

## ⚙️ How to Run  

### 1️⃣ Configure MongoDB  

Update `appsettings.json`:

```json
"MongoDbSettings": {
  "ConnectionString": "mongodb://localhost:27017",
  "DatabaseName": "DocuMindAI"
}
```

---

### 2️⃣ Run Backend (API)

```
dotnet run --project SimpleUplode
```

---

### 3️⃣ Run Frontend (MVC)

```
dotnet run --project MVCSimpleUplode
```

---

## 🐳 Run with Docker  

```
docker-compose up --build
```

---

## 📌 Major API Endpoints  

### Upload Document  
POST `/api/document/upload`

### Ask AI  
POST `/api/ai/ask`

Example:

```json
{
  "question": "Explain .NET Developer role",
  "documentId": "document-id"
}
```

---

## 🎯 Key Highlights  

✔ Full Stack Architecture  
✔ AI-Powered Document Q&A  
✔ Secure API Design  
✔ MongoDB NoSQL Integration  
✔ Vector Search using Qdrant  
✔ Subscription-Based Feature Control  

---

## 👨‍💻 Author  

**Amit Dange**  
.NET Developer | AI Integration | MongoDB  
