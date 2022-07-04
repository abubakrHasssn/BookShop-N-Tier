using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookShop.DataAccess.Data;
using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models;

namespace BookShop.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Product product)
        {
            var productFromDb = _context.Products.FirstOrDefault(p=> p.Id == product.Id);
            if (productFromDb != null)
            {
                productFromDb.Title = product.Title;
                productFromDb.ISBN =product.ISBN;
                productFromDb.Price =product.Price;
                productFromDb.ListPrice =product.ListPrice;
                productFromDb.Price50 =  product.Price50;    
                productFromDb.Price100 = product.Price100;
                productFromDb.Description= product.Description;
                productFromDb.CategoryId = product.CategoryId;
                productFromDb.CoverTypeId = product.CoverTypeId;
                productFromDb.Author = product.Author;
                if (product.ImageUrl != null)
                {
                    productFromDb.ImageUrl = product.ImageUrl;
                }
                
            }
        }
    }
}
