# LootHub.Api

REST API for **LootHub**, an e-commerce site for gaming gear, built with **ASP.NET Core (.NET 10)** following **Clean Architecture**.
Customers browse products, manage their addresses, place orders and pay through **iyzico** (sandbox). Admins manage the catalog and orders, including cancellations and returns with refunds.

This API is consumed by a separate ASP.NET Core MVC client (`LootHub.Client`).

## Tech stack

| Area | Technology |
| --- | --- |
| Framework | ASP.NET Core Web API, .NET 10 |
| Data access | Entity Framework Core 10, SQL Server (LocalDB) |
| Auth | ASP.NET Core Identity, JWT bearer tokens, role-based authorization |
| Payments | iyzico Checkout Form (sandbox) via the `Iyzipay` package, plus a fake gateway for local testing |
| Validation | FluentValidation |
| API docs | OpenAPI + Scalar |

## Architecture

```
LootHub.Domain           Entities, value objects (Money, Address), enums, BaseEntity. No dependencies.
LootHub.Application      DTOs, validators, services, Result pattern, repository and gateway interfaces
LootHub.Infrastructure   EF Core DbContext, configurations, migrations, repositories, Unit of Work,
                         JWT token service, payment gateways (iyzico + fake)
LootHub.WebApi           Controllers, Program.cs, JWT bearer setup, seeders
```

Dependencies point inward: `WebApi → Application ← Infrastructure`, and everything depends on `Domain`.
Interfaces (ports) live in Application; Infrastructure provides the implementations (adapters).

### Highlights

- **Server-side pricing.** Clients send only product ids and quantities. Prices, product names, customer details and the shipping address are read on the server and copied into the order as a snapshot, so later changes to a product or address never change an existing order.
- **Ownership checks.** The user id always comes from the token (`sub` claim), never from the URL or body. Every customer query filters by user id; another user's address or order returns `404`, not `403`, so the API does not reveal that it exists.
- **Payment abstraction.** `IPaymentGateway` (initialize, verify, refund) has two implementations: `IyzicoPaymentGateway` and `FakePaymentGateway`. The provider is chosen by configuration; `OrderService` did not change when iyzico was added.
- **Stock is reduced when payment is confirmed**, not when the order is placed, so unpaid orders do not lock stock. Stock is checked again during verification, and a `CHECK (Stock >= 0)` constraint guards against concurrent payments.
- **Idempotent payment verification.** A payment can be verified only once; repeating the request returns `409` and does not reduce stock twice.
- **Order state machine.** Allowed admin transitions are whitelisted (`Paid → Shipped → Delivered`). Cancelling a paid order or returning a delivered one restocks the items and refunds the payment through the gateway.
- **Soft delete and audit fields.** Entities carry `IsActive`, `CreatedAtUtc`, `LastModifiedAtUtc` and `DeletedAtUtc`. Deleted rows are hidden by a global query filter, and unique indexes are filtered on active rows. `LastModifiedAtUtc` is set centrally in `SaveChangesAsync`.
- **Value objects.** `Money` and `Address` are records mapped as EF owned types.
- **Result pattern.** Services return `Result` / `Result<T>` with an `ErrorType`; a base controller maps them to `400 / 401 / 402 / 404 / 409` responses in `ProblemDetails` format.

## Getting started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server LocalDB (included with Visual Studio). Any SQL Server instance works if you change the connection string.
- EF Core CLI tools: `dotnet tool install --global dotnet-ef`

### 1. Clone

```bash
git clone https://github.com/batueren0/LootHub.Api.git
cd LootHub.Api
```

### 2. Configure secrets

Secrets are **not** stored in the repository. Set them with User Secrets:

```bash
dotnet user-secrets set "Jwt:SecretKey" "<a random string of at least 32 characters>" --project LootHub.WebApi
dotnet user-secrets set "Seed:AdminPassword" "<a password that meets the policy below>" --project LootHub.WebApi
```

Password policy: at least 8 characters, with an uppercase letter, a lowercase letter, a digit and a non-alphanumeric character.

### 3. Choose a payment provider (optional)

By default the API uses the **fake** gateway, so it runs without any payment account. To use iyzico sandbox, create a sandbox account at https://sandbox-merchant.iyzipay.com, then:

```bash
dotnet user-secrets set "Iyzico:ApiKey" "sandbox-..." --project LootHub.WebApi
dotnet user-secrets set "Iyzico:SecretKey" "sandbox-..." --project LootHub.WebApi
dotnet user-secrets set "Payments:Provider" "Iyzico" --project LootHub.WebApi
```

If `Payments:Provider` is `Iyzico` and the keys are missing, the application stops at startup with a clear error.

Non-secret settings (JWT issuer and audience, token lifetime, admin email, iyzico base URL and callback URL) are in `LootHub.WebApi/appsettings.json`.

### 4. Create the database

```bash
dotnet ef database update --project LootHub.Infrastructure --startup-project LootHub.WebApi
```

### 5. Run

```bash
dotnet run --project LootHub.WebApi --launch-profile https
```

Open **https://localhost:7149/scalar** for the interactive API reference.

On startup the app seeds an `Admin` role and an admin user (`admin@loothub.com`, password from User Secrets). In the Development environment it also seeds sample categories, brands, properties and products. Seeding is idempotent.

## Authentication

1. `POST /api/auth/register` (first name, last name, phone `5xxxxxxxxx`, email, password) or `POST /api/auth/login`.
2. Copy the `token` from the response.
3. Send it on protected requests: `Authorization: Bearer <token>`

| Situation | Response |
| --- | --- |
| No token, invalid token or expired token | `401 Unauthorized` |
| Valid token, but the role is not allowed | `403 Forbidden` |

Tokens are valid for 60 minutes and carry the `sub`, `email`, `name` and `role` claims.

## Endpoints

🔒 = requires login, 🛡️ = requires the `Admin` role

### Auth

| Method | Route | Description |
| --- | --- | --- |
| POST | `/api/auth/register` | Register a new user, returns a token |
| POST | `/api/auth/login` | Log in, returns a token |

### Products

| Method | Route | Description |
| --- | --- | --- |
| GET | `/api/products?search=&categoryId=&brandId=&minPrice=&maxPrice=&inStockOnly=&sort=&page=&pageSize=` | Paged list with filters. `sort`: `Newest`, `PriceAsc`, `PriceDesc`, `NameAsc` |
| GET | `/api/products/{slug}` | Product details with brand, category and properties |
| POST 🛡️ | `/api/products` | Create a product |
| PUT 🛡️ | `/api/products/{id}` | Update a product and sync its properties |
| DELETE 🛡️ | `/api/products/{id}` | Soft delete |
| PATCH 🛡️ | `/api/products/{id}/stock` | `Renew`, `Increase` or `Decrease` stock |

### Categories · Brands · Properties

All three follow the same pattern (`/api/categories`, `/api/brands`, `/api/properties`):

| Method | Route | Description |
| --- | --- | --- |
| GET | `/api/{resource}` | List all |
| GET | `/api/{resource}/{id}` | Get by id |
| POST 🛡️ | `/api/{resource}` | Create |
| PUT 🛡️ | `/api/{resource}/{id}` | Update |
| DELETE 🛡️ | `/api/{resource}/{id}` | Soft delete (blocked while products still reference it) |

### Addresses (current user)

| Method | Route | Description |
| --- | --- | --- |
| GET 🔒 | `/api/addresses` | My addresses |
| GET 🔒 | `/api/addresses/{id}` | One of my addresses |
| POST 🔒 | `/api/addresses` | Add an address (title must be unique per user) |
| PUT 🔒 | `/api/addresses/{id}` | Update |
| DELETE 🔒 | `/api/addresses/{id}` | Soft delete |

### Orders (current user)

| Method | Route | Description |
| --- | --- | --- |
| POST 🔒 | `/api/orders` | Place an order: `{ "addressId", "items": [{ "productId", "quantity" }] }` |
| GET 🔒 | `/api/orders` | My orders, newest first |
| GET 🔒 | `/api/orders/{orderNumber}` | Order details |
| POST 🔒 | `/api/orders/{orderNumber}/payments` | Start a payment, returns `token` and `paymentUrl` |
| POST 🔒 | `/api/orders/{orderNumber}/payments/verify` | Verify a payment: `{ "token" }` |
| POST 🔒 | `/api/orders/{orderNumber}/cancel` | Cancel an order that is still awaiting payment |

### Admin orders

| Method | Route | Description |
| --- | --- | --- |
| GET 🛡️ | `/api/admin/orders?status=&search=&page=&pageSize=` | All orders, paged; search by order number or customer name |
| GET 🛡️ | `/api/admin/orders/{orderNumber}` | Any order's details |
| PATCH 🛡️ | `/api/admin/orders/{orderNumber}/status` | `{ "status": "Shipped" }` or `"Delivered"` |
| POST 🛡️ | `/api/admin/orders/{orderNumber}/cancel` | Cancel; a paid order is restocked and refunded |
| POST 🛡️ | `/api/admin/orders/{orderNumber}/return` | Return a delivered order: restock and refund |

## Order and payment flow

```
PendingPayment ──pay──▶ Paid ──▶ Shipped ──▶ Delivered ──return──▶ Returned
      │                   │
      └──cancel──▶ Cancelled ◀──cancel (admin, refund)
```

1. `POST /api/orders` creates the order as `PendingPayment`. Stock is checked but not reduced.
2. `POST .../payments` asks the gateway to start a payment and stores it as `Pending`. With iyzico, the customer is redirected to `paymentUrl` (iyzico's hosted page), so card data never reaches this API.
3. After paying, iyzico posts the token to `Iyzico:CallbackUrl` (handled by the MVC client).
4. `POST .../payments/verify` asks the gateway for the result. On success, stock is re-checked and reduced, the payment becomes `Completed` and the order `Paid`, all in one `SaveChanges`. A declined card returns `402`.

### Testing payments

- **Fake gateway:** payments above **100,000 TRY** are declined; everything else succeeds.
- **iyzico sandbox:** test card `5526 0800 0000 0006`, any future expiry date, any CVC. Use `4111 1111 1111 1129` for a declined payment (insufficient funds).

## Known limitations

- iyzico requires a national identity number; this project does not collect one and sends the sandbox test value `11111111111`.
- Basket items are sent to iyzico with a fixed category (`Gaming Gear`).
- iyzico's `fraudStatus` (manual review) is not handled.
- Users without a phone number (the seeded admin) cannot pay with iyzico.
- If two customers pay for the last unit at the same moment, the database rejects the second stock update and the API returns `409`, but that customer's payment has already been taken and must be refunded manually.
- Refunds always cover the whole payment; partial refunds are not supported.

## License

This project was built for learning purposes as the final project of the Infotech software course.
