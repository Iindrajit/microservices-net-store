using Microsoft.AspNetCore.Http;

namespace Shopping.Web.Pages
{
    public class OrderListModel
        (IOrderingService orderingService, ILogger<OrderListModel> logger)
        : PageModel
    {
        public IEnumerable<OrderModel> Orders { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            var customerId = Guid.Parse(User.Claims.FirstOrDefault(u => u.Type == "custId")?.Value!);

            var response = await orderingService.GetOrdersByCustomer(customerId);
            Orders = response.Orders;

            return Page();
        }
    }
}