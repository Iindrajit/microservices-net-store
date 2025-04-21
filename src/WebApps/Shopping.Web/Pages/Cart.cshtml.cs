namespace Shopping.Web.Pages
{
    public class CartModel(IBasketService basketService, ILogger<CartModel> logger)
        : PageModel
    {
        public ShoppingCartModel Cart { get; set; } = new ShoppingCartModel();

        public async Task<IActionResult> OnGetAsync()
        {
            var username = User.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            Cart = await basketService.LoadUserBasket(username);

            return Page();
        }

        public async Task<IActionResult> OnPostRemoveFromCartAsync(Guid productId)
        {
            logger.LogInformation("Remove from cart button clicked");

            var username = User.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            var Cart = await basketService.LoadUserBasket(username);

            Cart.Items.RemoveAll(x => x.ProductId == productId);

            await basketService.StoreBasket(new StoreBasketRequest(Cart));

            return RedirectToPage();
        }
    }
}
