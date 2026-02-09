namespace VendingMachineApp.Models;

public class Product
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    // Price in integer coins
    public int Price { get; set; }
    public int Quantity { get; set; }
}
