namespace Basket.API.Basket.GetBasket;

//public record GetBasketRequest(string UserName); 
public record GetBasketResponse(ShoppingCart Cart);

// Defines an API endpoint for retrieving a user's shopping basket.
// Implements ICarterModule to register routes in a modular way.
public class GetBasketEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // Maps a GET request to "/basket/{userName}" that retrieves the user's shopping basket.
        app.MapGet("/basket/{userName}", async (string userName, ISender sender) =>
        {
            // Sends a query to fetch the basket for the specified user.
            var result = await sender.Send(new GetBasketQuery(userName));

            // Maps the result to a response DTO.
            var response = result.Adapt<GetBasketResponse>();

            // Returns an HTTP 200 response with the basket data.
            return Results.Ok(response);
        })
        //These are extension methods provided by ASP.NET Core Minimal APIs
        //They are fluent (chaining) extension methods because they are extension methods
        //that allow method chaining, making the code more readable and expressive
        .RequireAuthorization() // Ensures that only authenticated users can access this endpoint.
        .WithName("GetBasketByUser") // Assigns a name to the endpoint for routing purposes.
        .ProducesProblem(StatusCodes.Status401Unauthorized) // Defines possible responses, including authentication failures.
        .Produces<GetBasketResponse>(StatusCodes.Status200OK) // Defines a successful response format.
        .ProducesProblem(StatusCodes.Status400BadRequest) // Specifies response for invalid requests.
        .WithSummary("Get User's Basket")
        .WithDescription("Retrieves the shopping basket for a specified user");
    }
}
