namespace Basket.API.Models;

public class ShoppingCartItem
{
    public int Quantity { get; set; } = default!;
    public string Color { get; set; } = default!;
    public decimal Price { get; set; } = default!; // Final price after discount
    public decimal OriginalPrice { get; set; } = default!; // Store original price
    public Guid ProductId { get; set; } = default!;
    public string ProductName { get; set; } = default!;
}
