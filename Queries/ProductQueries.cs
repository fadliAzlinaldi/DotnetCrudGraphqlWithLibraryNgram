using DotnetGraphQLCRUD.Model;
using Library.Ngram;
using Library.Ngram.Model;
using Microsoft.EntityFrameworkCore;

namespace DotnetGraphQLCRUD.Queries
{
    public class ProductQueries
    {
        private readonly IElasticsearchServices _elasticsearchServices;

        public ProductQueries(IElasticsearchServices elasticsearchServices)
        {
            _elasticsearchServices = elasticsearchServices;
        }

        public Product GetProductById(string id)
        {
            try
            {
                var result = _elasticsearchServices.GetDocumentById<Product>(id);
                return result;
            }
            catch
            {
                throw new GraphQLException($"Failed to fetch products, product not found");
            }
        }

        public List<Product> GetAllProduct()
        {
            var result = _elasticsearchServices.GetAllDocuments<Product>();
            return result;
        }

        public IEnumerable<Product> SearchProducts(string searchTerm)
        {
            return _elasticsearchServices.SearchWithNGram<Product>(searchTerm, "name");
        }
    }
}
