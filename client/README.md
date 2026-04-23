# ExploreIO

A real-time messaging application with user profiles and social following. Built with ASP.NET Core and Angular.

## Overview

ExploreIO enables real-time direct messaging between users with persistent message storage, user presence tracking, and profile management. Users can discover and follow other members, exchange messages instantly via WebSockets, and manage their conversations with soft deletion support.

## Features

- **Real-Time Messaging**
  - Instant message delivery via SignalR WebSockets
  - Direct messaging between users
  - Message persistence and soft deletion
  - Message thread management with pagination

- **User Authentication**
  - JWT-based authentication
  - Role-based access control
  - ASP.NET Core Identity integration

- **User Profiles & Discovery**
  - User profiles with photos and bio information
  - Follow/unfollow system
  - User search and browsing
  - Online presence tracking

- **Media Management**
  - Profile photo uploads via Cloudinary
  - Multi-photo support per user

## System Architecture

The application uses a client-server architecture with real-time communication:

**Frontend (Angular SPA)**
- REST API calls for CRUD operations
- SignalR WebSocket connections for real-time messaging and presence
- JWT authentication via HTTP interceptors
- Route guards for protected routes

**Backend (ASP.NET Core Web API)**
- RESTful endpoints for user management, messages, and follows
- SignalR hubs for bidirectional real-time communication
- Repository pattern with Unit of Work for data access
- JWT middleware for authentication

**Data Flow**
1. HTTP requests → Controllers → Repositories → EF Core → PostgreSQL
2. Database changes → SignalR hubs → WebSocket broadcast → Connected clients
3. Real-time events (new messages, presence updates) pushed via WebSockets

**Communication**
- REST API for CRUD operations
- SignalR WebSockets for messaging and presence
- JWT tokens for stateless authentication

## Tech Stack

### Frontend
- **Framework:** Angular 12
- **UI Libraries:** Bootstrap 4, Bootswatch, ngx-bootstrap, Font Awesome
- **Real-Time:** @microsoft/signalr
- **Utilities:** ngx-toastr (notifications), ngx-spinner (loading), ngx-timeago (timestamps), ng2-file-upload
- **State Management:** RxJS

### Backend
- **Framework:** ASP.NET Core (.NET 8)
- **ORM:** Entity Framework Core 8
- **Authentication:** ASP.NET Core Identity, JWT Bearer tokens
- **Real-Time:** SignalR
- **Mapping:** AutoMapper
- **Cloud Services:** Cloudinary (image hosting)

### Database
- **Primary:** PostgreSQL (production)
- **Development:** SQLite (optional for local development)

### Development Tools
- **API Testing:** Postman
- **Version Control:** Git

## Project Structure

```
ExploreIO/
│
├── API/                          
│   ├── Controllers/              # API endpoints
│   │   ├── AccountController.cs  # Authentication (login, register)
│   │   ├── MessagesController.cs # Message CRUD operations
│   │   ├── UsersController.cs    # User profiles and browsing
│   │   ├── FollowingsController.cs # Follow/unfollow operations
│   │   └── AdminController.cs    # Admin-only operations
│   │
│   ├── SignalR/                  # Real-time hubs
│   │   ├── MessageHub.cs         # Real-time messaging
│   │   ├── PresenceHub.cs        # User presence tracking
│   │   └── PresenceTracker.cs    # Online users tracking
│   │
│   ├── Data/                     # Data access layer
│   │   ├── DataContext.cs        # EF Core DbContext
│   │   ├── Migrations/           # Database migrations
│   │   ├── *Repository.cs        # Repository implementations
│   │   └── UnitOfWork.cs         # Unit of Work pattern
│   │
│   ├── Entities/                 # Domain models
│   │   ├── AppUser.cs            # User entity
│   │   ├── Message.cs            # Message entity
│   │   ├── UserFollow.cs         # Follow relationship
│   │   └── Photo.cs              # Photo entity
│   │
│   ├── DTOs/                     # Data transfer objects
│   ├── Interfaces/               # Service contracts
│   ├── Services/                 # Business logic services
│   ├── Middleware/               # Custom middleware (exception handling)
│   ├── Extensions/               # Service configuration extensions
│   └── Helpers/                  # Utilities (pagination, automapper profiles)
│
└── client/                       
    └── src/
        └── app/
            ├── _guards/          # Route guards (auth, admin)
            ├── _interceptors/    # HTTP interceptors (JWT, error handling)
            ├── _services/        # API client services
            ├── _models/          # TypeScript interfaces
            ├── _resolvers/       # Route resolvers
            ├── members/          # User profile components
            ├── messages/         # Messaging components
            ├── followings/       # Following/followers components
            ├── nav/              # Navigation component
            └── admin/            # Admin panel components
```

## Getting Started

### Prerequisites

- **.NET SDK 8.0** or higher
- **Node.js 14.x** or higher (with npm)
- **PostgreSQL 12** or higher
- **Angular CLI** (`npm install -g @angular/cli@12`)
- **Cloudinary Account** (for image uploads)

### Installation

#### 1. Clone the Repository
```bash
git clone https://github.com/YahiaHelal/ExploreIO.git
cd ExploreIO
```

#### 2. Backend Setup

```bash
cd API

# Restore dependencies
dotnet restore

# Update appsettings.Development.json with your database connection string
# (see Environment Variables section below)

# Apply database migrations
dotnet ef database update

# Run the API
dotnet run
```

The API will start at `https://localhost:5001` (or as configured in `launchSettings.json`).

#### 3. Frontend Setup

```bash
cd client

# Install dependencies
npm install

# Start the development server
npm start
```

The Angular app will start at `https://localhost:4200`.

### Running Both Services

For local development:
1. Start the backend API first (Terminal 1): `cd API && dotnet run`
2. Start the frontend (Terminal 2): `cd client && npm start`
3. Navigate to `https://localhost:4200` in your browser

## Environment Variables

### Backend (`API/appsettings.Development.json`)

Create or update the `appsettings.Development.json` file:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=5432;User Id=YOUR_DB_USER;Password=YOUR_DB_PASSWORD;Database=exploreio"
  },
  "TokenKey": "YOUR_SUPER_SECRET_JWT_KEY_MINIMUM_32_CHARACTERS",
  "CloudinarySettings": {
    "CloudName": "YOUR_CLOUDINARY_CLOUD_NAME",
    "ApiKey": "YOUR_CLOUDINARY_API_KEY",
    "ApiSecret": "YOUR_CLOUDINARY_API_SECRET"
  }
}
```

**Environment Variables:**
- `ConnectionStrings:DefaultConnection` — PostgreSQL connection string (or SQLite for development)
- `TokenKey` — Secret key for JWT signing (min 32 characters for production)
- `CloudinarySettings:CloudName` — Cloudinary cloud name for image hosting
- `CloudinarySettings:ApiKey` — Cloudinary API key
- `CloudinarySettings:ApiSecret` — Cloudinary API secret

### Frontend (`client/src/environments/environment.ts`)

```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:5001/api/',
  hubUrl: 'https://localhost:5001/hubs/'
};
```

## API Overview

### Authentication Endpoints
- `POST /api/account/register` — Register new user
- `POST /api/account/login` — Login and receive JWT token

### User Endpoints
- `GET /api/users` — Get paginated list of users (with filters)
- `GET /api/users/{username}` — Get user profile by username
- `PUT /api/users` — Update current user's profile

### Message Endpoints
- `GET /api/messages` — Get paginated messages (inbox/outbox)
- `POST /api/messages` — Send a new message
- `DELETE /api/messages/{id}` — Soft delete a message

### Following Endpoints
- `GET /api/followings` — Get user's followers or following list
- `POST /api/followings/{username}` — Follow a user
- `DELETE /api/followings/{username}` — Unfollow a user

### SignalR Hubs
- `/hubs/presence` — Real-time presence tracking
- `/hubs/message` — Real-time message delivery

All endpoints except `/account/register` and `/account/login` require JWT authentication via `Authorization: Bearer <token>` header.

## Known Issues / Limitations

- **Legacy Node.js Setup:** The frontend requires `NODE_OPTIONS=--openssl-legacy-provider` due to Angular 12 and older OpenSSL compatibility issues.
- **No Message Editing:** Messages can only be deleted (soft deletion), not edited after sending.
- **No Email Verification:** User registration does not include email verification.
- **CORS Configuration:** Hardcoded to `https://localhost:4200` in development. Requires update for production deployment.

## Future Improvements

- Message editing and reactions
- Group messaging and channels
- Read receipts and typing indicators
- Message search functionality
- Voice/video calling (WebRTC)
- End-to-end encryption
- Mobile applications (iOS/Android)

## Contributing

Contributions are welcome! Please follow these guidelines:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/your-feature-name`)
3. Commit your changes with clear commit messages
4. Push to your branch (`git push origin feature/your-feature-name`)
5. Open a Pull Request with a detailed description of changes

### Code Standards
- Follow existing code style and conventions
- Add comments for complex logic
- Update documentation for new features
- Test your changes locally before submitting
