# Shopping Cart API

A well-structured RESTful API demonstrating clean architecture principles, SOLID design, and test-driven development using ASP.NET Core 8.

## Table of Contents

- [Features](#features)
- [Architecture & Design](#architecture--design)
- [Technology Stack](#technology-stack)
- [Getting Started](#getting-started)
- [API Endpoints](#api-endpoints)
- [Testing](#testing)
- [Design Decisions](#design-decisions)

<a name="features"></a>
## Features

✅ **Add items to cart** - Add products with validation  
✅ **Update item quantity** - Modify quantities for existing items  
✅ **Remove items** - Delete items from the cart  
✅ **View cart items** - Retrieve all cart items with summary  
✅ **Cart summary** - Real-time totals and statistics  
✅ **Input validation** - Comprehensive request validation  
✅ **Error handling** - Consistent error responses  
✅ **Interactive UI** - Static HTML interface for testing

<a name="architecture--design"></a>
## Architecture & Design

### Project Structure

```
ShoppingCart/
├── ShoppingCart.API/          # Web API Layer
│   ├── Controllers/           # API Controllers
│   ├── Services/              # Business Logic
│   ├── Models/                # Domain Models
│   ├── DTOs/                  # Data Transfer Objects
│   ├── Common/                # Shared utilities (Result pattern)
│   └── wwwroot/               # Static files (UI)
├── ShoppingCart.Test/         # Unit Tests
└── ShoppingCart.sln           # Solution file
```

### Design Patterns Used

#### 1. **Result Pattern**
Instead of throwing exceptions for business logic failures, the application uses a `Result<T>` type that encapsulates both success and failure states. This approach:
- Makes error handling explicit and type-safe
- Improves performance by avoiding exception overhead
- Makes the code more functional and predictable
- Provides clear error codes and messages

```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Data { get; }
    public Error? Error { get; }
}
```

#### 2. **Domain-Driven Design (DDD)**
The `CartItem` model follows DDD principles:
- **Private constructor** prevents invalid object creation
- **Factory method** (`Create`) ensures validation at creation
- **Encapsulation** - all fields are private with controlled mutations
- **Business logic in the domain** - `UpdateQuantity` and `TotalPrice` live in the model

#### 3. **Dependency Injection**
The controller depends on `ICartService` interface, not the concrete implementation:
- Enables unit testing with mocks
- Follows the Dependency Inversion Principle (SOLID)
- Makes the code flexible for future changes (e.g., adding database persistence)

#### 4. **Single Responsibility Principle**
Each class has one clear responsibility:
- **CartController** - HTTP concerns and routing
- **CartService** - Business logic and cart state management
- **CartItem** - Domain rules and validation
- **DTOs** - Data validation and transfer

### SOLID Principles Applied

- **S**ingle Responsibility: Each class has one reason to change
- **O**pen/Closed: Services can be extended without modifying existing code
- **L**iskov Substitution: ICartService implementations are interchangeable
- **I**nterface Segregation: Clean, focused interface definition
- **D**ependency Inversion: Controllers depend on abstractions, not concretions

<a name="technology-stack"></a>
## Technology Stack

- **Framework**: .NET 8.0
- **Web Framework**: ASP.NET Core 8.0
- **Testing**: xUnit, FluentAssertions, NSubstitute
- **API Documentation**: Swagger/OpenAPI
- **Code Coverage**: Coverlet

<a name="getting-started"></a>
## Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio 2022 / VS Code / Rider (optional)

### Running the Application

1. **Clone the repository**
```bash
git clone <repository-url>
cd ShoppingCart
```

2. **Restore dependencies**
```bash
dotnet restore
```

3. **Run the API**
```bash
cd ShoppingCart.API
dotnet run
```

The API will start on:
- HTTP: `http://localhost:5170`
- HTTPS: `https://localhost:7077`

4. **Access Swagger UI**
Open browser to: `https://localhost:7077/swagger`

5. **Access Web UI**
Open browser to: `https://localhost:7077/cart-index.html`

### Running Tests

```bash
# Run all tests
dotnet test

# Run with code coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=lcov
```
<a name="api-endpoints"></a>
## API Endpoints

### Base URL: `/api/cart`

#### 1. Get All Cart Items
```http
GET /api/cart
```

**Response** (200 OK):
```json
{
  "hasItems": true,
  "message": "Cart items retrieved successfully",
  "items": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "productName": "Running Shoes",
      "price": 89.99,
      "quantity": 2,
      "addedAt": "2026-02-09T10:30:00Z",
      "totalPrice": 179.98
    }
  ],
  "summaryDto": {
    "uniqueItems": 1,
    "totalQuantity": 2,
    "totalAmount": 179.98
  }
}
```

#### 2. Add Item to Cart
```http
POST /api/cart
Content-Type: application/json

{
  "productName": "Running Shoes",
  "price": 89.99,
  "quantity": 2
}
```

**Response** (200 OK):
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "productName": "Running Shoes",
  "price": 89.99,
  "quantity": 2,
  "addedAt": "2026-02-09T10:30:00Z",
  "totalPrice": 179.98
}
```

**Validation Rules**:
- Product name: Required, 1-100 characters
- Price: Required, > 0
- Quantity: Required, 1-1000

#### 3. Update Item Quantity
```http
PUT /api/cart/{itemId}
Content-Type: application/json

{
  "quantity": 5
}
```

**Response** (200 OK):
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "productName": "Running Shoes",
  "price": 89.99,
  "quantity": 5,
  "addedAt": "2026-02-09T10:30:00Z",
  "totalPrice": 449.95
}
```

**Error Response** (404 Not Found):
```json
{
  "code": "NOT_FOUND",
  "message": "Item not found in cart."
}
```

#### 4. Remove Item from Cart
```http
DELETE /api/cart/{itemId}
```

**Response** (204 No Content)

**Error Response** (404 Not Found):
```json
{
  "code": "NOT_FOUND",
  "message": "Item not found in cart."
}
```

<a name="testing"></a>
## Testing

The project includes comprehensive unit tests covering:

### CartItemTests
- Domain model validation
- Business rule enforcement
- Edge cases for pricing and quantity

### CartServiceTests
- All CRUD operations
- Error scenarios
- Cart summary calculations
- Complete workflow testing

### CartControllerTests
- HTTP response codes
- Error handling
- Integration with service layer

**Test Coverage**: 95%+ across all layers

### Running Specific Tests

```bash
# Run service tests only
dotnet test --filter "FullyQualifiedName~CartServiceTests"

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"
```
<a name="design-decisions"></a>
## Design Decisions

### Why No Database?

For this coding challenge, I used in-memory storage (`List<CartItem>`) to:
- Focus on architecture and clean code
- Simplify setup and evaluation
- Demonstrate patterns that work regardless of persistence layer

**Production Migration Path**: The current design makes it trivial to add database persistence:
1. Keep the same interface (`ICartService`)
2. Create new implementation (e.g., `EfCoreCartService`)
3. Register in DI container
4. No controller changes needed

### Why Singleton Service?

The `CartService` is registered as a Singleton to maintain cart state across requests. This simulates a session-scoped cart without introducing session complexity.

**Production Consideration**: In a real application, you would:
- Use database persistence with user IDs
- Implement proper session management
- Add authentication/authorization
- Consider distributed caching (Redis)

### Why Result Pattern Over Exceptions?

**Benefits**:
- **Performance**: No stack unwinding overhead
- **Explicit**: Forces callers to handle errors
- **Functional**: More predictable control flow
- **Testable**: Easier to test error scenarios

**Trade-off**: Slightly more verbose than exceptions, but the benefits outweigh the cost.

### Why DTOs Separate from Models?

**Separation Benefits**:
- **API Stability**: Change internal models without breaking API contracts
- **Validation**: Input validation separate from domain validation
- **Security**: Don't expose internal IDs or sensitive fields
- **Flexibility**: API can evolve independently of domain

### Input Validation Strategy

The project uses **two layers of validation**:

1. **DTO Validation** (Data Annotations): Validates request shape and basic rules
2. **Domain Validation** (Business Logic): Validates business rules in the model

This defense-in-depth approach catches issues early while maintaining clean domain models.

## Future Enhancements

If this were a production system, would add:

### Immediate Priorities
- [ ] Database persistence (EF Core + SQL Server)
- [ ] User authentication (JWT/OAuth)
- [ ] User-specific carts (multi-user support)
- [ ] Shopping cart expiration/cleanup
- [ ] Logging and monitoring (Serilog, Application Insights)

## Project Highlights

### Code Patterns Demonstrated

- Result/Either pattern for error handling
- Factory pattern for object creation
- Repository pattern (interface separation)
- DTO pattern for API contracts
- Strategy pattern (interface-based services)

## License

This project is created as a coding challenge demonstration.

## Contact

**Samuel Mbah**  
Email: samuel.mbah@email.com  
GitHub: https://github.com/samuelcmbah

---

**Note**: This is a demonstration project for a coding assessment. It showcases clean code principles, testing practices, and architectural thinking suitable for production e-commerce systems.
