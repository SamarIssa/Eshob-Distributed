using Catalog.Models;
using Catalog.Services;

namespace Catalog.Endpoints
{
    public static class ProductEndpoints
    {
        public static void MapProductEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/products").WithTags("Products");
            group.MapGet("/", async (ProductService productService) =>
            {
                var products = await productService.GetProductsAsync();
                return Results.Ok(products);
            })
            .WithName("GetProducts")
            .Produces<IEnumerable<Product>>(StatusCodes.Status200OK);

            group.MapGet("/{id:int}", async (int id, ProductService productService) =>
            {
                var product = await productService.GetProductByIdAsync(id);
                return product is not null ? Results.Ok(product) : Results.NotFound();
            })
            .WithName("GetProductById")
            .Produces<Product>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

            group.MapPost("/", async (Product product, ProductService productService) =>
            {
                await productService.CreateProductAsync(product);
                return Results.Created($"/products/{product.Id}", product);
            })
            .WithName("CreateProduct")
            .Produces<Product>(StatusCodes.Status201Created);

            group.MapPut("/{id:int}", async (int id, Product inputProduct, ProductService productService) =>
            {
                var updatedProduct = await productService.GetProductByIdAsync(id);
                if (updatedProduct is null)
                    return Results.NotFound();
                await productService.UpdateProductAsync(updatedProduct, inputProduct);
                return Results.NoContent();
            })
            .WithName("UpdateProduct")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

            group.MapDelete("/{id:int}", async (int id, ProductService productService) =>
            {
                var deletedProduct = await productService.GetProductByIdAsync(id);
                if (deletedProduct is null)
                    return Results.NotFound();
                await productService.DeleteProductAsync(deletedProduct);
                return Results.NoContent();
            })
            .WithName("DeleteProduct")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
        }
    }
}
