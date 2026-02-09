using VendingMachineApp.Models;

namespace VendingMachineApp.Services;

public class SelectionResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Product? Product { get; set; }
    public int Change { get; set; }
}

public class VendingMachine
{
    private readonly object _lock = new();
    private readonly Dictionary<string, Product> _inventory = new();
    // coin balance (integer coins)
    public int CoinBalance { get; private set; }

    public VendingMachine()
    {
        // seed three beverages, price is two quarters (2 coins) and quantity 5 each
        AddStock(new Product { Code = "A1", Name = "Soda", Price = 2, Quantity = 5 });
        AddStock(new Product { Code = "B1", Name = "Iced Coffee", Price = 2, Quantity = 5 });
        AddStock(new Product { Code = "C1", Name = "Iced Tea", Price = 2, Quantity = 5 });
    }

    public IEnumerable<Product> GetProducts()
    {
        lock (_lock)
        {
            return _inventory.Values.Select(p => new Product { Code = p.Code, Name = p.Name, Price = p.Price, Quantity = p.Quantity }).ToList();
        }
    }

    // Accepts only US quarters represented as coin == 1 (one quarter)
    public bool AcceptCoin(int coin)
    {
        if (coin != 1) return false;
        lock (_lock)
        {
            CoinBalance += 1;
            return true;
        }
    }

    public int ReturnCoins()
    {
        lock (_lock)
        {
            var toReturn = CoinBalance;
            CoinBalance = 0;
            return toReturn;
        }
    }

    public (bool success, int coinsToReturn, int quantityVended, string message, int remaining) Purchase(string code)
    {
        lock (_lock)
        {
            if (!_inventory.TryGetValue(code, out var product))
            {
                var refund = CoinBalance;
                CoinBalance = 0;
                return (false, refund, 0, "Product not found", 0);
            }
            if (product.Quantity <= 0)
            {
                // out of stock - refund all coins
                var accepted = CoinBalance;
                CoinBalance = 0;
                return (false, accepted, 0, "Out of stock", 0);
            }
            if (CoinBalance < product.Price)
            {
                // insufficient coins - refund current coins
                var refund = CoinBalance;
                CoinBalance = 0;
                return (false, refund, 0, "Insufficient coins", product.Quantity);
            }

            // Dispense single beverage
            product.Quantity -= 1;
            var coinsToReturn = CoinBalance - product.Price;
            CoinBalance = 0;
            return (true, coinsToReturn, 1, "Vended", product.Quantity);
        }
    }

    public void AddStock(Product product)
    {
        lock (_lock)
        {
            if (_inventory.TryGetValue(product.Code, out var existing))
            {
                existing.Quantity += product.Quantity;
                existing.Price = product.Price;
                existing.Name = product.Name;
            }
            else
            {
                _inventory[product.Code] = new Product { Code = product.Code, Name = product.Name, Price = product.Price, Quantity = product.Quantity };
            }
        }
    }

    public bool RemoveStock(string code, int quantity)
    {
        lock (_lock)
        {
            if (!_inventory.TryGetValue(code, out var p)) return false;
            if (quantity <= 0) return false;
            if (p.Quantity < quantity) return false;
            p.Quantity -= quantity;
            return true;
        }
    }

    public void Reset()
    {
        lock (_lock)
        {
            _inventory.Clear();
            CoinBalance = 0;
        }
    }
}
