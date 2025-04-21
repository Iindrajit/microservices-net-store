namespace Shopping.Web.Pages
{
    public class CheckoutModel
        (IBasketService basketService, ILogger<CheckoutModel> logger)
        : PageModel
    {
        [BindProperty]
        public BasketCheckoutModel Order { get; set; } = default!;
        public ShoppingCartModel Cart { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            var username = User.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            Cart = await basketService.LoadUserBasket(username);

            return Page();
        }

        public async Task<IActionResult> OnPostCheckOutAsync()
        {
            logger.LogInformation("Checkout button clicked");

            var username = User.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            Cart = await basketService.LoadUserBasket(username);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            Order.CustomerId = Guid.Parse(User.Claims.FirstOrDefault(u => u.Type == "custId")?.Value!);
            Order.UserName = Cart.UserName;
            Order.TotalPrice = Cart.TotalPrice;

            await basketService.CheckoutBasket(new CheckoutBasketRequest(Order));

            return RedirectToPage("Confirmation", "OrderSubmitted");
        }
    }
}