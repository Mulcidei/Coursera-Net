# Blog API

A simple yet robust RESTful API for managing blog posts built with ASP.NET Core Minimal APIs. The API demonstrates best practices including middleware for error handling, token validation, and request logging.

## Features

- ✅ **CRUD Operations**: Create, read, update, and delete blog posts
- ✅ **Token-Based Authentication**: Secure endpoints with Bearer token validation
- ✅ **Error Handling Middleware**: Centralized exception handling with JSON error responses
- ✅ **Request Logging Middleware**: Log HTTP methods, paths, and response status codes
- ✅ **RESTful Design**: Clean API endpoints following REST conventions
- ✅ **In-Memory Data Storage**: Quick setup with sample blog data

## Prerequisites

- .NET 10.0 or higher
- Visual Studio Code (recommended)
- REST Client extension for VS Code (for testing)

## Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd UserManagementApi
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the project**
   ```bash
   dotnet build
   ```

## Running the Application

Start the development server:

```bash
dotnet run
```

The API will be available at `http://localhost:5077` (or as configured in `launchSettings.json`)

## API Endpoints

### Get All Blogs
```http
GET /blogs
Authorization: Bearer <token>
```
**Response**: `200 OK` - Returns array of all blog posts

### Get Blog by ID
```http
GET /blogs/{id}
Authorization: Bearer <token>
```
**Response**: `200 OK` - Returns specific blog post or `404 Not Found`

### Create Blog
```http
POST /blogs
Authorization: Bearer <token>
Content-Type: application/json

{
  "title": "Blog Title",
  "body": "Blog content"
}
```
**Response**: `201 Created` - Returns created blog with location header

### Update Blog
```http
PUT /blogs/{id}
Authorization: Bearer <token>
Content-Type: application/json

{
  "title": "Updated Title",
  "body": "Updated content"
}
```
**Response**: `200 OK` - Returns updated blog or `404 Not Found`

### Delete Blog
```http
DELETE /blogs/{id}
Authorization: Bearer <token>
```
**Response**: `204 No Content` - Blog deleted successfully or `404 Not Found`

## Authentication

All endpoints require a valid Bearer token in the `Authorization` header.

**Format**: `Authorization: Bearer <token>`

**Valid Token Example**: `Bearer valid-secret-token-12345`

**Invalid requests** will return:
```json
{
  "error": "Unauthorized: Missing token."
}
```
or
```json
{
  "error": "Unauthorized: Invalid token format."
}
```

## Middleware

### 1. Error Handling Middleware (Executed First)
- Catches unhandled exceptions
- Returns consistent JSON error responses
- Example error response:
  ```json
  {
    "error": "Internal server error."
  }
  ```

### 2. Token Validation Middleware (Executed Second)
- Validates Bearer tokens from incoming requests
- Returns `401 Unauthorized` for missing or invalid tokens
- Requires token format: `Bearer <token>`

### 3. Logging Middleware (Executed Last)
- Logs every request with:
  - HTTP method (GET, POST, PUT, DELETE)
  - Request path
  - Response status code
- Example log: `HTTP GET /blogs - Response: 200`

**Middleware Order** (Important):
```
Request → Error Handling → Token Validation → Logging → Route Handler → Response
```

## Testing

### Using REST Client Extension (VS Code)

1. Install the **REST Client** extension in VS Code
2. Open [Request.http](Request.http)
3. Click "Send Request" on any test case
4. View responses in the side panel

### Sample Test Cases

- **Test 1**: Request without token (401 Unauthorized)
- **Test 2**: Request with invalid token (401 Unauthorized)
- **Test 3-12**: CRUD operations with valid token

### Using curl

```bash
# Get all blogs
curl -H "Authorization: Bearer valid-secret-token-12345" \
     http://localhost:5077/blogs

# Create a blog
curl -X POST http://localhost:5077/blogs \
     -H "Authorization: Bearer valid-secret-token-12345" \
     -H "Content-Type: application/json" \
     -d '{"title":"New Blog","body":"Content"}'

# Get blog by ID
curl -H "Authorization: Bearer valid-secret-token-12345" \
     http://localhost:5077/blogs/0

# Update blog
curl -X PUT http://localhost:5077/blogs/0 \
     -H "Authorization: Bearer valid-secret-token-12345" \
     -H "Content-Type: application/json" \
     -d '{"title":"Updated","body":"Updated content"}'

# Delete blog
curl -X DELETE http://localhost:5077/blogs/0 \
     -H "Authorization: Bearer valid-secret-token-12345"
```

## Project Structure

```
UserManagementApi/
├── Program.cs                          # Main application file with endpoints and middleware
├── Request.http                        # REST API test cases
├── appsettings.json                    # Application settings
├── appsettings.Development.json        # Development-specific settings
├── UserManagementApi.csproj            # Project file
├── Properties/
│   └── launchSettings.json             # Launch configuration
├── obj/                                # Build artifacts
├── bin/                                # Compiled output
└── README.md                           # This file
```

## Data Model

### Blog Class
```csharp
public class Blog
{
    public required string Title { get; set; }
    public required string Body { get; set; }
}
```

## Configuration

The application uses default ASP.NET Core configuration. Key settings can be modified in:
- `appsettings.json` - Default settings
- `appsettings.Development.json` - Development-specific settings

## Logs

Logs are output to the console during development. Examples:
```
info: LoggingMiddleware[0] HTTP GET /blogs - Response: 200
warn: TokenValidationMiddleware[0] Missing authorization token
info: TokenValidationMiddleware[0] Valid token provided
```

## Future Enhancements

- Database integration (Entity Framework Core)
- User authentication with JWT tokens
- Pagination for blog listings
- Search and filtering capabilities
- Blog categories and tags
- Comments system
- Unit and integration tests

## License

This project is open source and available under the MIT License.

## Contributing

Contributions are welcome! Please feel free to submit pull requests or open issues for bugs and feature requests.

## Support

For questions or issues, please open an issue on the GitHub repository.
