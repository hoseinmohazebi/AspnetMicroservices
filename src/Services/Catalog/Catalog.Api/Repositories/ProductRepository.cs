using Catalog.Api.Data;
using Catalog.Api.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Api.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ICatalogContext _context;

        public ProductRepository(ICatalogContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            var filter = Builders<Product>.Filter.Where(e => true); // new ver
            return await _context.Products.FindSync(filter).ToListAsync();
        }

        public async Task<Product> GetProduct(string id)
        {
            var filter = Builders<Product>.Filter.Where(e => e.Id == id); // new ver
            return await _context.Products.FindSync(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Product>> GetProductByName(string name)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.Name, name); // new ver
            return await _context.Products.FindSync(filter).ToListAsync();
        }
        public async Task<IEnumerable<Product>> GetProductByCategory(string categoryName)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.Category, categoryName); // new ver
           var z = await _context.Products.Find(filter).ToListAsync();
            return await _context.Products.FindSync(filter).ToListAsync();
        }

        public async Task CreateProduct(Product product)
        {
            await _context.Products.InsertOneAsync(product);
        }

        public async Task<bool> UpdateProduct(Product product)
        {
            var filter = Builders<Product>.Filter
                .ElemMatch(p => p.Id, product.Id); // new ver

            var update = Builders<Product>.Update
                .Set(t => t.ImageFile, product.ImageFile)
                .Set(t => t.Name, product.Name)
                .Set(t => t.Price, product.Price)
                .Set(t => t.Summary, product.Summary)
                .Set(t => t.Category, product.Category)
                .Set(t => t.Description, product.Description);

            var updateRes = await _context.Products
                .UpdateOneAsync(filter, update);

            return updateRes.IsAcknowledged && updateRes.ModifiedCount > 0;
        }

        public async Task<bool> DeleteProduct(string id)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Id, id);

            DeleteResult deleteResult = await _context
                                                .Products
                                                .DeleteOneAsync(filter);

            return deleteResult.IsAcknowledged
                && deleteResult.DeletedCount > 0;
        }
    }
}
