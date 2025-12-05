# Interview API - .NET Core 8.0 Web API

A complete, runnable .NET Core 8.0 Web API project that demonstrates common interview concepts and best practices.

## Features Demonstrated

### 1. **CRUD Operations**
- Full CRUD endpoints for Candidate entity
- Pagination and sorting support
- Custom search functionality

### 2. **Architecture Patterns**
- **Repository Pattern**: Separation of data access logic
- **Service Layer**: Business logic separation
- **Dependency Injection**: Proper IoC container usage
- **DTO Pattern**: Request/Response DTOs using record types

### 3. **Exception Handling**
- Global exception handler using `IExceptionHandler`
- Custom exceptions (`NotFoundException`, `ValidationException`)
- Consistent error response format

### 4. **Filters & Attributes**
- **TimingActionFilter**: Logs request duration
- **ValidationFilterAttribute**: Model validation before action execution
- **ResultFilterAttribute**: Response formatting

### 5. **Middleware**
- Custom rate limiting middleware (in-memory)
- Request tracking and throttling

### 6. **Advanced Features**
- Async/Await pattern throughout
- LINQ operations for data querying
- Extension methods demonstration
- DataAnnotations for model validation
- Health check endpoint
- Swagger/OpenAPI documentation
- Serilog for logging
- IOptions pattern for configuration

## Project Structure

```
InterviewAPI/
├── Controllers/
│   └── CandidateController.cs      # API endpoints
├── DTOs/
│   ├── CreateCandidateRequest.cs   # Request DTOs
│   ├── UpdateCandidateRequest.cs
│   ├── CandidateResponse.cs        # Response DTOs
│   └── PagedResponse.cs            # Pagination DTO
├── Models/
│   └── Candidate.cs                # Domain model
├── Repositories/
│   ├── ICandidateRepository.cs     # Repository interface
│   └── CandidateRepository.cs      # Repository implementation
├── Services/
│   ├── ICandidateService.cs        # Service interface
│   └── CandidateService.cs         # Service implementation
├── Exceptions/
│   ├── NotFoundException.cs        # Custom exceptions
│   ├── ValidationException.cs
│   └── GlobalExceptionHandler.cs   # Global exception handler
├── Filters/
│   ├── TimingActionFilter.cs       # Action filters
│   ├── ValidationFilterAttribute.cs
│   └── ResultFilter.cs
├── Middleware/
│   └── RateLimitingMiddleware.cs   # Custom middleware
├── Extensions/
│   ├── ApiSettings.cs              # Configuration
│   └── CandidateExtensions.cs     # Extension methods
├── Program.cs                      # Application entry point
└── InterviewAPI.csproj             # Project file
```

## Setup Instructions

### Prerequisites
- .NET 8.0 SDK installed
- Terminal/Command Prompt

### Steps to Run

1. **Navigate to project directory**
   ```bash
   cd InterviewAPI
   ```

2. **Restore packages** (if needed)
   ```bash
   dotnet restore
   ```

3. **Run the application**
   ```bash
   dotnet run
   ```

4. **Access Swagger UI**
   - Open browser and navigate to: `https://localhost:5001/swagger` or `http://localhost:5000/swagger`
   - The exact port will be shown in the console output

5. **Health Check**
   - Navigate to: `https://localhost:5001/health` or `http://localhost:5000/health`

## API Endpoints

### Base URL
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`

### Endpoints

#### 1. Get All Candidates (with pagination & sorting)
```
GET /api/candidates?pageNumber=1&pageSize=10&sortBy=firstName&sortOrder=asc
```

#### 2. Get Candidate by ID
```
GET /api/candidates/{id}
```

#### 3. Create Candidate
```
POST /api/candidates
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "phone": "+1234567890",
  "experienceYears": 5,
  "skills": ["C#", ".NET", "ASP.NET Core"]
}
```

#### 4. Update Candidate
```
PUT /api/candidates/{id}
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "phone": "+1234567890",
  "experienceYears": 6,
  "skills": ["C#", ".NET", "ASP.NET Core", "Docker"]
}
```

#### 5. Delete Candidate
```
DELETE /api/candidates/{id}
```

#### 6. Search by Skill
```
GET /api/candidates/search?skill=C#
```

## Interview Concepts Explained

### 1. **Repository Pattern**
- Abstracts data access logic
- Makes code testable and maintainable
- Located in `Repositories/` folder

### 2. **Service Layer**
- Contains business logic
- Handles validation and orchestration
- Located in `Services/` folder

### 3. **Dependency Injection**
- Configured in `Program.cs`
- Services registered with appropriate lifetimes
- Constructor injection used throughout

### 4. **Global Exception Handling**
- `IExceptionHandler` implementation
- Centralized error handling
- Consistent error responses

### 5. **Filters**
- Action filters for cross-cutting concerns
- Timing, validation, and result formatting

### 6. **Middleware**
- Custom rate limiting middleware
- Request/response pipeline manipulation

### 7. **Async/Await**
- All repository and service methods are async
- Non-blocking I/O operations

### 8. **LINQ**
- Used extensively for querying and sorting
- Demonstrates functional programming concepts

### 9. **Extension Methods**
- Utility methods for Candidate operations
- Demonstrates C# language features

### 10. **DTOs with Records**
- Immutable data transfer objects
- Type-safe request/response models

## Testing the API

### Using Swagger UI
1. Navigate to Swagger UI (shown in console output)
2. Use the interactive interface to test endpoints
3. View request/response schemas

### Using curl

**Create a candidate:**
```bash
curl -X POST "https://localhost:5001/api/candidates" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "Jane",
    "lastName": "Smith",
    "email": "jane.smith@example.com",
    "phone": "+1234567890",
    "experienceYears": 3,
    "skills": ["JavaScript", "React", "Node.js"]
  }'
```

**Get all candidates:**
```bash
curl -X GET "https://localhost:5001/api/candidates?pageNumber=1&pageSize=10"
```

**Search by skill:**
```bash
curl -X GET "https://localhost:5001/api/candidates/search?skill=React"
```

## Notes

- **In-Memory Storage**: Data is stored in memory and will be lost when the application stops
- **Rate Limiting**: Limited to 100 requests per minute per IP address
- **No Database**: Uses Dictionary/List for storage as per requirements
- **Configuration**: All settings configured in-memory via `IOptions` pattern
- **Logging**: Serilog configured for console output only

## Key Interview Points

When discussing this project in interviews, highlight:

1. **Separation of Concerns**: Clear layers (Controller → Service → Repository)
2. **SOLID Principles**: Single Responsibility, Dependency Inversion
3. **Design Patterns**: Repository, Service Layer, Dependency Injection
4. **Error Handling**: Global exception handling with custom exceptions
5. **Performance**: Async/await, efficient data structures
6. **Maintainability**: Clean code, proper naming, comments
7. **API Design**: RESTful principles, proper HTTP verbs
8. **Testing Readiness**: Interfaces enable easy mocking for unit tests

## License

This project is created for educational and interview demonstration purposes.

