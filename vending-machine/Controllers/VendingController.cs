using Microsoft.AspNetCore.Mvc;
using VendingMachineApp.Models;
using VendingMachineApp.Services;

namespace VendingMachineApp.Controllers;

[ApiController]
public class VendingController : ControllerBase
{
    private readonly VendingMachine _vm;

    public VendingController(VendingMachine vm)
    {
        _vm = vm;
    }

    // PUT /  -> insert coin
    [HttpPut("/")]
    public IActionResult InsertCoin([FromBody] CoinRequest req)
    {
        if (req == null || req.Coin <= 0) return BadRequest();
        _vm.AcceptCoin(req.Coin);
        Response.Headers["X-Coins"] = req.Coin.ToString();
        return NoContent();
    }

    // DELETE / -> return coins
    [HttpDelete("/")]
    public IActionResult ReturnCoins()
    {
        var returned = _vm.ReturnCoins();
        Response.Headers["X-Coins"] = returned.ToString();
        return NoContent();
    }

    // GET /inventory -> array of remaining quantities
    [HttpGet("/inventory")]
    public ActionResult<int[]> GetInventory()
    {
        var products = _vm.GetProducts().OrderBy(p => p.Code).ToArray();
        var quantities = products.Select(p => p.Quantity).ToArray();
        return Ok(quantities);
    }

    // GET /inventory/{id} -> remaining quantity integer
    [HttpGet("/inventory/{id}")]
    public ActionResult<int> GetInventoryItem(string id)
    {
        var product = _vm.GetProducts().FirstOrDefault(p => p.Code == id);
        if (product == null) return NotFound();
        return Ok(product.Quantity);
    }

    // PUT /inventory/{id} -> attempt purchase
    [HttpPut("/inventory/{id}")]
    public IActionResult Purchase(string id)
    {
        var (success, coinsToReturn, quantityVended, message, remaining) = _vm.Purchase(id);

        if (!success && (message == "Out of stock" || message == "Product not found"))
        {
            Response.Headers["X-Coins"] = coinsToReturn.ToString();
            return StatusCode(404);
        }
        if (!success && message == "Insufficient coins")
        {
            Response.Headers["X-Coins"] = coinsToReturn.ToString();
            return StatusCode(403);
        }

        Response.Headers["X-Coins"] = coinsToReturn.ToString();
        Response.Headers["X-Inventory-Remaining"] = remaining.ToString();
        return Ok(new { quantity = quantityVended });
    }
}
