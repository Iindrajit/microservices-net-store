namespace Shopping.Web.Pages;
public class IndexModel
    (ICatalogService catalogService, IBasketService basketService, ILogger<IndexModel> logger)
    : PageModel
{
    public IEnumerable<ProductModel> ProductList { get; set; } = new List<ProductModel>();    

    public async Task<IActionResult> OnGetAsync()
    {
        logger.LogInformation("Index page visited");
        var result = await catalogService.GetProducts();

        if (result.Products != null)
        {
            ProductList = result.Products;
        }
        else
        {
            ViewData["FallbackMessage"] = "Oops, something went wrong. Please try again later.";
        }

        //ProductList = result.Products;
        return Page();
    }

    public async Task<IActionResult> OnPostAddToCartAsync(Guid productId)
    {
        logger.LogInformation("Add to cart button clicked");
        var productResponse = await catalogService.GetProduct(productId);

        var username = User.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
        var basket = await basketService.LoadUserBasket(username);

        basket.Items.Add(new ShoppingCartItemModel
        {
            ProductId = productId,
            ProductName = productResponse.Product.Name,
            Price = productResponse.Product.Price,
            Quantity = 1,
            Color = "Black",
            OriginalPrice = productResponse.Product.Price
        });

        await basketService.StoreBasket(new StoreBasketRequest(basket));

        return RedirectToPage("Cart");
    }
}
