namespace Shopping.Web.Services;

public interface ICatalogService
{
    [Get("/catalogs?pageNumber={pageNumber}&pageSize={pageSize}")]
    Task<GetProductsResponse> GetProducts(int? pageNumber = 1, int? pageSize = 10);

    [Get("/catalogs/{id}")]
    Task<GetProductByIdResponse> GetProduct(Guid id);

    [Get("/catalogs/category/{category}")]
    Task<GetProductByCategoryResponse> GetProductsByCategory(string category);
}
