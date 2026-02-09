# Vending Machine (minimal .NET API)

This is a minimal vending machine project scaffolded as a .NET 8 minimal API.

Getting started

Prerequisites: .NET 8 SDK installed.

Run:

```bash
cd vending-machine
dotnet run
```

API endpoints

#### Coins
- `PUT /` — insert coin
  - Body: `{ "coin": <integer> }`
  - Response: `204 No Content`, header `X-Coins` contains inserted amount

- `DELETE /` — return all coins
  - Response: `204 No Content`, header `X-Coins` contains total returned

#### Inventory
- `GET /inventory` — list all product quantities
  - Response: `200 OK`, body: array of integers (quantities in order by product code)

- `GET /inventory/{id}` — get quantity for specific product
  - Response: `200 OK`, body: `<integer>`
  - Returns: `404 Not Found` if product not exists

- `PUT /inventory/{id}` — attempt purchase of product
  - Response: `200 OK` on success, body: `{ "quantity": <integer> }`, headers: `X-Coins` (coins returned), `X-Inventory-Remaining` (quantity left)
  - Returns: `403 Forbidden` if insufficient coins inserted (header `X-Coins` shows coins to return)
  - Returns: `404 Not Found` if product out of stock or not found (header `X-Coins` shows coins to return)

Notes

- Persistence is in-memory. Add a DB if you need durable state.
- Includes simple inventory management.
