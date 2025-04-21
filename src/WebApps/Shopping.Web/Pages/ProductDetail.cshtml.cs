namespace Shopping.Web.Pages
{
    public class ProductDetailModel
        (ICatalogService catalogService, IBasketService basketService, ILogger<ProductDetailModel> logger)
        : PageModel
    {
        public ProductModel Product { get; set; } = default!;

        [BindProperty]
        public string Color { get; set; } = default!;

        [BindProperty]
        public int Quantity { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid productId)
        {
            var response = await catalogService.GetProduct(productId);
            Product = response.Product;

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
                Quantity = Quantity,
                Color = Color,
                OriginalPrice = productResponse.Product.Price
            });

            await basketService.StoreBasket(new StoreBasketRequest(basket));

            return RedirectToPage("Cart");
        }
    }
}