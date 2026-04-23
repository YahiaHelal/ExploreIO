# ExploreIO

A fullstack real-time chat application with user profiles, messaging, and social following features. Built with ASP.NET Core and Angular.

## Overview

ExploreIO is a social messaging platform that enables users to create profiles, discover other users, follow them, and exchange real-time messages. The application provides a responsive interface for browsing members, managing connections, and maintaining conversations with pagination and presence tracking.

**Target Users:** Developers and teams looking for a reference implementation of a modern fullstack chat system with authentication, real-time communication, and social features.

## Features

- **User Authentication & Authorization**
  - JWT-based authentication
  - Role-based access control (user and admin roles)
  - Identity management with ASP.NET Core Identity

- **Real-Time Messaging**
  - Direct messaging between users
  - Message persistence with soft deletion
  - Real-time delivery via SignalR WebSockets
  - Message thread management

- **Social Features**
  - User profiles with photos and personal information
  - Follow/unfollow functionality
  - User discovery and browsing
  - Activity tracking (last active timestamps)

- **Media Handling**
  - Profile photo uploads
  - Cloudinary integration for image storage
  - Photo management per user

- **User Experience**
  - Pagination for messages, users, and followers
  - Real-time presence tracking
  - Responsive UI with Bootstrap and Bootswatch themes
  - Toast notifications and loading indicators

## System Architecture

### High-Level Overview

The application follows a client-server architecture with clear separation of concerns:

**Frontend (Angular SPA)**
- Communicates with the backend via HTTP REST API for CRUD operations
- Establishes WebSocket connections through SignalR hubs for real-time features (messaging, presence)
- Uses route guards for authentication and authorization
- Implements interceptors for JWT token management and error handling

**Backend (ASP.NET Core Web API)**
- RESTful API following standard HTTP conventions
- Repository pattern with Unit of Work for data access
- SignalR hubs for real-time bidirectional communication
- JWT middleware for request authentication
- AutoMapper for DTO transformations

**Data Flow**
1. Client sends HTTP requests to REST endpoints (authentication, CRUD operations)
2. Controllers validate requests and delegate to repositories via Unit of Work
3. Repositories interact with Entity Framework Core DbContext
4. Database changes trigger SignalR hub notifications to connected clients
5. Real-time updates flow through WebSocket connections back to clients

**Communication Patterns**
- **REST API**: Standard CRUD operations, pagination, filtering
- **SignalR WebSockets**: Real-time messaging, presence updates, notifications
- **JWT Authentication**: Stateless token-based auth across all requests

## Tech Stack

### Frontend
- **Framework:** Angular 12
- **UI Libraries:** Bootstrap 4, Bootswatch, ngx-bootstrap, Font Awesome
- **Real-Time:** @microsoft/signalr
- **Utilities:** ngx-toastr (notifications), ngx-spinner (loading), ngx-timeago (timestamps), ng2-file-upload
- **State Management:** RxJS

### Backend
- **Framework:** ASP.NET Core (.NET 10)
- **ORM:** Entity Framework Core 10
- **Authentication:** ASP.NET Core Identity, JWT Bearer tokens
- **Real-Time:** SignalR
- **Mapping:** AutoMapper
- **Cloud Services:** Cloudinary (image hosting)

### Database
- **Primary:** PostgreSQL (production)
- **Development:** SQLite (optional for local development)

### Development Tools
- **API Documentation:** Swagger/OpenAPI
- **Version Control:** Git

## Project Structure

```
ExploreIO/
│
├── API/                          # Backend ASP.NET Core project
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
└── client/                       # Frontend Angular application
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

- **.NET SDK 10.0** or higher
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

- **Date/Time Handling:** The `LastActive` timestamp may display incorrect future times in some timezones (noted in `AppUser.cs`). This is a known issue with UTC conversion.
- **Legacy Node.js Setup:** The frontend requires `NODE_OPTIONS=--openssl-legacy-provider` due to Angular 12 and older OpenSSL compatibility issues.
- **No Message Editing:** Messages can only be deleted (soft deletion), not edited after sending.
- **Single Photo Upload:** Users can upload multiple photos, but there's no built-in UI for batch uploads.
- **No Email Verification:** User registration does not include email verification.
- **CORS Configuration:** Hardcoded to `https://localhost:4200` in development. Requires update for production deployment.

## Future Improvements

- [ ] Implement message editing functionality
- [ ] Add email verification and password reset flows
- [ ] Implement group chat/channels
- [ ] Add read receipts and typing indicators
- [ ] Migrate to Angular 15+ for better performance and modern tooling
- [ ] Add end-to-end encryption for messages
- [ ] Implement comprehensive unit and integration tests
- [ ] Add Redis caching for frequently accessed data
- [ ] Implement full-text search for users and messages
- [ ] Add WebRTC support for voice/video calls
- [ ] Dockerize the application for easier deployment
- [ ] Implement CI/CD pipeline

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

## License

This project does not currently have a specified license. Please contact the repository owner for usage permissions.

---

**Repository:** [https://github.com/YahiaHelal/ExploreIO](https://github.com/YahiaHelal/ExploreIO)