using DotnetGraphQLCRUD.Model;
using Library.Ngram;

namespace DotnetGraphQLCRUD.Mutation
{
    public class MutationProduct
    {
        private readonly IElasticsearchServices _elasticsearchServices;

        public MutationProduct(IElasticsearchServices elasticsearchServices)
        {
            _elasticsearchServices = elasticsearchServices;
        }
        public async Task<Product> AddProduct([Service] ApplicationDbContext context, ProductInput input)
        {
            var product = new Product
            {
                Name = input.Name,
                Description = input.Description,
                Category = input.Category
            };
            context.Products.Add(product);
            await context.SaveChangesAsync();


            var cek = product;
            _elasticsearchServices.IndexDocument(product);
            return product;
        }

        public async Task<Product> UpdateProduct([Service] ApplicationDbContext context, int id, ProductInput input)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var product = await context.Products.FindAsync(id);

                if (product == null)
                {
                    throw new Exception("Product not found");
                }

                product.Name = input.Name;
                product.Description = input.Description;
                product.Category = input.Category;

                context.Products.Update(product);
                await context.SaveChangesAsync();

                
                _elasticsearchServices.UpdateDocument(product.Id.ToString(), product);

                await transaction.CommitAsync();

                return product;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApplicationException($"Failed to update product: {ex.Message}", ex);
            }
        }

        public async Task<string> DeleteProduct([Service] ApplicationDbContext context, int productId)
        {
            var product = await context.Products.FindAsync(productId);
            if (product == null)
            {
                return "Product not found";
            }

            context.Products.Remove(product);
            await context.SaveChangesAsync();

            _elasticsearchServices.DeleteDocument(productId.ToString());

            return "Product Deleted";


        }

        public IEnumerable<Product> SearchProductsWithPagination(string searchTerm, int pageNumber, int pageSize)
        {
            return _elasticsearchServices.SearchWithPagination<Product>(searchTerm, "name", pageNumber, pageSize);
        }
    }
}
